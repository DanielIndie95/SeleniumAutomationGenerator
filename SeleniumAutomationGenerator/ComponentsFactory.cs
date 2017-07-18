using System.Collections.Generic;
using System.Linq;
using Core;
using Core.Models;
using Core.Utils;
using SeleniumAutomationGenerator.Generator.ClassAppenders;
using SeleniumAutomationGenerator.Generator.ComponentsGenerators;
using SeleniumAutomationGenerator.Generator.PropertyGenerators;
using SeleniumAutomationGenerator.Models;
using SeleniumAutomationGenerator.Utils;

namespace SeleniumAutomationGenerator
{
    public sealed class ComponentsFactory : IComponentsFactory
    {
        readonly Dictionary<string, IComponentFileCreator> _fileCreators;
        readonly Dictionary<string, IComponentClassAppender> _classAppenders;

        private readonly IComponentFileCreator _defaultFileCreator;

        public static ComponentsFactory Instance { get; }

        private ComponentsFactory()
        {
            _fileCreators = new Dictionary<string, IComponentFileCreator>();
            _classAppenders = new Dictionary<string, IComponentClassAppender>();
            AddComponentClassGeneratorKey("page", new BasicPageGenerator(new DriverFindElementPropertyGenerator(Consts.DRIVER_FIELD_NAME), Consts.PAGES_NAMESPACE));
            AddComponentClassGeneratorKey("model", new BasicModelGenerator(new ParentElementFindElementPropertyGenerator(Consts.DRIVER_FIELD_NAME, Consts.PARENT_ELEMENT_FIELD_NAME), Consts.PAGES_NAMESPACE, Consts.PARENT_ELEMENT_FIELD_NAME));

            _defaultFileCreator = new BasicComponentGenerator(new ParentElementFindElementPropertyGenerator(Consts.DRIVER_FIELD_NAME, Consts.PARENT_ELEMENT_FIELD_NAME), Consts.PAGES_NAMESPACE, Consts.PARENT_ELEMENT_FIELD_NAME);

            AddComponentTypeAppenders("list", new ListClassAppender());
        }

        static ComponentsFactory()
        {
            Instance = new ComponentsFactory();
        }

        public void AddComponentClassGeneratorKey(string key, IComponentFileCreator newComponentFileCreator)
        {
            _fileCreators[key] = newComponentFileCreator;

        }

        public void AddComponentTypeAppenders(string type, IComponentClassAppender classAppender)
        {
            _classAppenders[type] = classAppender;
            _defaultFileCreator.AddExceptionPropertyType(type);
            foreach (var fileCreator in _fileCreators)
            {
                fileCreator.Value.AddExceptionPropertyType(type);
            }
        }
        public IEnumerable<ComponentGeneratorOutput> CreateCsOutput(string body)
        {
            IEnumerable<AutoElementData> children = AutoElementFinder.GetChildren(body);
            IEnumerable<ComponentGeneratorOutput> totalChildren = new List<ComponentGeneratorOutput>();
            foreach (var child in children)
            {
                totalChildren = totalChildren.Union(CreateCsOutput(child.Selector, child.InnerChildrens), new ComponentOutputComparer());
            }
            return totalChildren;
        }

        private IEnumerable<ComponentGeneratorOutput> CreateCsOutput(string selector, IEnumerable<AutoElementData> children, IComponentFileCreator parentClassCreator = null)
        {
            var keyWord = SelectorUtils.GetKeyWordFromSelector(selector);
            IEnumerable<AutoElementData> autoElementDatas = children as AutoElementData[] ?? children.ToArray();
            if (!autoElementDatas.Any()) //not a new cs file
                return new List<ComponentGeneratorOutput>();

            IEnumerable<AutoElementData> filteredChildren = autoElementDatas
                .Where(FilterNonInlineChidren);
            IEnumerable<ElementSelectorData> childrenData = filteredChildren
                .Select(ConvertToElementSelectorData);
            ElementSelectorData[] elements = TransformFileCreatorsToAddinsLike(childrenData).ToArray();
            if (_classAppenders.ContainsKey(keyWord) && parentClassCreator != null)
            {
                HandleClassAppenders(selector, parentClassCreator, keyWord, elements);
                IEnumerable<ComponentGeneratorOutput> outputs = new List<ComponentGeneratorOutput>();
                foreach (var child in filteredChildren)
                {
                    outputs = outputs.Concat(CreateCsOutput(child.Selector, child.InnerChildrens));
                }
                return outputs;
            }
            return GetFileCreatorsOutput(selector, autoElementDatas, keyWord, elements);
        }

        private void HandleClassAppenders(string selector, IComponentFileCreator parentClassCreator, string keyWord, IEnumerable<ElementSelectorData> childrenData)
        {
            _classAppenders[keyWord].AppendToClass(parentClassCreator, selector, childrenData.ToArray());
        }

        private IEnumerable<ComponentGeneratorOutput> GetFileCreatorsOutput(string selector, IEnumerable<AutoElementData> children, string keyWord, ElementSelectorData[] childrenData)
        {
            IEnumerable<ComponentGeneratorOutput> outputs = new List<ComponentGeneratorOutput>();
            IComponentFileCreator parent = _fileCreators.ContainsKey(keyWord) ? _fileCreators[keyWord] : _defaultFileCreator;
            foreach (var child in children)
            {
                outputs = outputs.Union(CreateCsOutput(child.Selector, child.InnerChildrens.ToList(), parent), new ComponentOutputComparer());
            }            
            ComponentGeneratorOutput parentOutput = parent.GenerateComponentClass(selector, childrenData);
            outputs = outputs.Union(new[] { parentOutput }, new ComponentOutputComparer());
            return outputs;
        }

        private IEnumerable<ElementSelectorData> TransformFileCreatorsToAddinsLike(IEnumerable<ElementSelectorData> childrenData)
        {
            foreach (var child in childrenData)
            {
                if (_fileCreators.TryGetValue(child.Type, out IComponentFileCreator fileCreator))
                {
                    ComponentsContainer.Instance.AddAddin(fileCreator.MakeAddin(child.FullSelector));
                    child.Type = child.Name;
                }
                yield return child;
            }
        }
    
        private bool FilterNonInlineChidren(AutoElementData childData)
        {
            string keyWord = SelectorUtils.GetKeyWordFromSelector(childData.Selector);

            return !_classAppenders.ContainsKey(keyWord);
        }

        private static ElementSelectorData ConvertToElementSelectorData(AutoElementData data)
        {
            return new ElementSelectorData
            {
                FullSelector = data.Selector,
                Name = SelectorUtils.GetClassOrPropNameFromSelector(data.Selector),
                Type = SelectorUtils.GetKeyWordFromSelector(data.Selector),
                AutomationAttributes = data.AutoAttributes
            };
        }
    }
}
