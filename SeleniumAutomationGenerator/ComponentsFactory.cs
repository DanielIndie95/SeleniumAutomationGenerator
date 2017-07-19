using System;
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
        private readonly Dictionary<string, IComponentFileCreator> _fileCreators;
        private readonly Dictionary<string, IComponentClassAppender> _classAppenders;

        private readonly IComponentFileCreator _defaultFileCreator;

        public static ComponentsFactory Instance { get; }

        private ComponentsFactory()
        {
            _fileCreators = new Dictionary<string, IComponentFileCreator>();
            _classAppenders = new Dictionary<string, IComponentClassAppender>();
            AddComponentClassGeneratorKey("page",
                new BasicPageGenerator(new DriverFindElementPropertyGenerator(Consts.DRIVER_FIELD_NAME),
                    Consts.PAGES_NAMESPACE));
            AddComponentClassGeneratorKey("model",
                new BasicModelGenerator(
                    new ParentElementFindElementPropertyGenerator(Consts.DRIVER_FIELD_NAME,
                        Consts.PARENT_ELEMENT_FIELD_NAME), Consts.PAGES_NAMESPACE, Consts.PARENT_ELEMENT_FIELD_NAME));

            _defaultFileCreator = new BasicComponentGenerator(
                new ParentElementFindElementPropertyGenerator(Consts.DRIVER_FIELD_NAME,
                    Consts.PARENT_ELEMENT_FIELD_NAME), Consts.PAGES_NAMESPACE, Consts.PARENT_ELEMENT_FIELD_NAME);

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
            foreach (KeyValuePair<string, IComponentFileCreator> fileCreator in _fileCreators)
            {
                fileCreator.Value.AddExceptionPropertyType(type);
            }
        }

        public IEnumerable<ComponentGeneratorOutput> CreateCsOutput(string body)
        {
            IEnumerable<AutoElementData> children = AutoElementFinder.GetChildren(body);
            IEnumerable<ComponentGeneratorOutput> totalChildren = new List<ComponentGeneratorOutput>();
            foreach (AutoElementData child in children)
            {
                totalChildren =
                    totalChildren.Union(CreateCsOutput(child.Selector, child), new ComponentOutputComparer());
            }
            return totalChildren;
        }

        private IEnumerable<ComponentGeneratorOutput> CreateCsOutput(string selector, AutoElementData current,
            IComponentFileCreator parentClassCreator = null)
        {
            if (ContainsCustomAttributes(current) && parentClassCreator != null)
                RunAppendsOnParent(current, parentClassCreator);
            IEnumerable<AutoElementData> children = current.InnerChildrens;
            string keyWord = SelectorUtils.GetKeyWordFromSelector(selector);
            IEnumerable<AutoElementData> autoElementDatas = children as AutoElementData[] ?? children.ToArray();
            if (!autoElementDatas.Any()) //not a new cs file
                return new List<ComponentGeneratorOutput>();

            IEnumerable<AutoElementData> filteredChildren = autoElementDatas
                .Where(FilterNonInlineChidren);
            IEnumerable<ElementSelectorData> childrenData = filteredChildren
                .Select(ConversionsUtils.ConvertToElementSelectorData);
            ElementSelectorData[] elements = TransformFileCreatorsToAddinsLike(childrenData).ToArray();
            if (IsAppenderSuitable(parentClassCreator, keyWord))
            {
                return GenerateAppenderOutputs(selector, current, parentClassCreator, keyWord, filteredChildren);
            }

            return GetFileCreatorsOutput(selector, autoElementDatas, keyWord, elements);
        }
      
        private static void RunAppendsOnParent(AutoElementData current, IComponentFileCreator parentClassCreator)
        {
            IEnumerable<IElementAttribute> customAttributes = current.AutoAttributes
                .Select(att => ComponentsContainer.Instance.GetElementAttribute(att)).Where(att=> att!= null);
            foreach (IElementAttribute attribute in customAttributes)
            {
                attribute.AppendToClass(parentClassCreator , current);
            }
        }

        private static bool ContainsCustomAttributes(AutoElementData current)
        {
            return current.AutoAttributes.Any(att => ComponentsContainer.Instance.GetElementAttribute(att) != null);
        }

        private IEnumerable<ComponentGeneratorOutput> GenerateAppenderOutputs(string selector, AutoElementData current,
            IComponentFileCreator parentClassCreator, string keyWord, IEnumerable<AutoElementData> filteredChildren)
        {
            HandleClassAppenders(selector, parentClassCreator, keyWord, current);
            IEnumerable<ComponentGeneratorOutput> outputs = new List<ComponentGeneratorOutput>();
            foreach (var child in filteredChildren)
            {
                outputs = outputs.Concat(CreateCsOutput(child.Selector, child,parentClassCreator));
            }
            return outputs;
        }

        private bool IsAppenderSuitable(IComponentFileCreator parentClassCreator, string keyWord)
        {
            return _classAppenders.ContainsKey(keyWord) && parentClassCreator != null;
        }

        private void HandleClassAppenders(string selector, IComponentFileCreator parentClassCreator, string keyWord,
            AutoElementData element)
        {
            _classAppenders[keyWord].AppendToClass(parentClassCreator, element);
        }

        private IEnumerable<ComponentGeneratorOutput> GetFileCreatorsOutput(string selector,
            IEnumerable<AutoElementData> children, string keyWord, ElementSelectorData[] childrenData)
        {
            IEnumerable<ComponentGeneratorOutput> outputs = new List<ComponentGeneratorOutput>();
            IComponentFileCreator parent =
                _fileCreators.ContainsKey(keyWord) ? _fileCreators[keyWord] : _defaultFileCreator;
            foreach (var child in children)
            {
                outputs = outputs.Union(CreateCsOutput(child.Selector, child, parent), new ComponentOutputComparer());
            }
            ComponentGeneratorOutput parentOutput = parent.GenerateComponentClass(selector, childrenData);
            outputs = outputs.Union(new[] {parentOutput}, new ComponentOutputComparer());
            return outputs;
        }

        private IEnumerable<ElementSelectorData> TransformFileCreatorsToAddinsLike(
            IEnumerable<ElementSelectorData> childrenData)
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
    }
}