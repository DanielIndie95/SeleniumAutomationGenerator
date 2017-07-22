using System.Collections.Generic;
using System.Linq;
using Core;
using Core.Models;
using Core.Utils;
using SeleniumAutomationGenerator.Utils;

namespace SeleniumAutomationGenerator
{
    public class ComponentsFactory : IComponentsFactory
    {
        private readonly IFileCreatorContainer _fileCreatorContainer;
        private readonly IClassAppenderContainer _classAppenderContainer;
        private readonly IAddinContainer _addinContainer;
        private readonly IElementAttributeContainer _attributesContainer;

        public ComponentsFactory(IFileCreatorContainer fileCreatorContainer, IClassAppenderContainer classAppenderContainer, IAddinContainer addinContainer , IElementAttributeContainer attributesContainer)
        {
            _fileCreatorContainer = fileCreatorContainer;
            _classAppenderContainer = classAppenderContainer;
            _addinContainer = addinContainer;
            _attributesContainer = attributesContainer;
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

            AutoElementData[] filteredChildren = autoElementDatas
                .Where(FilterNonInlineChidren)
                .ToArray();
            IEnumerable<ElementSelectorData> childrenData = filteredChildren
                .Select(ConversionsUtils.ConvertToElementSelectorData);
            ElementSelectorData[] elements = TransformFileCreatorsToAddinsLike(childrenData).ToArray();
            if (IsAppenderSuitable(parentClassCreator, keyWord))
            {
                return GenerateAppenderOutputs(current, parentClassCreator, keyWord, filteredChildren);
            }

            return GetFileCreatorsOutput(selector, autoElementDatas, keyWord, elements);
        }

        private void RunAppendsOnParent(AutoElementData current, IComponentFileCreator parentClassCreator)
        {
            IEnumerable<IElementAttribute> customAttributes = current.AutoAttributes
                .Where(_attributesContainer.ContainsCustomAttribute)
                .Select(att => _attributesContainer.GetElementAttribute(att));

            foreach (IElementAttribute attribute in customAttributes)
            {
                attribute.AppendToClass(parentClassCreator, current);
            }
        }

        private bool ContainsCustomAttributes(AutoElementData current)
        {
            return current.AutoAttributes.Any(_attributesContainer.ContainsCustomAttribute);
        }

        private IEnumerable<ComponentGeneratorOutput> GenerateAppenderOutputs(AutoElementData current,
            IComponentFileCreator parentClassCreator, string keyWord, IEnumerable<AutoElementData> filteredChildren)
        {
            HandleClassAppenders(parentClassCreator, keyWord, current);
            IEnumerable<ComponentGeneratorOutput> outputs = new List<ComponentGeneratorOutput>();
            foreach (AutoElementData child in filteredChildren)
            {
                outputs = outputs.Concat(CreateCsOutput(child.Selector, child, parentClassCreator));
            }
            return outputs;
        }

        private bool IsAppenderSuitable(IComponentFileCreator parentClassCreator, string keyWord)
        {
            return _classAppenderContainer.ContainsAppender(keyWord)
                && parentClassCreator != null;
        }

        private void HandleClassAppenders(IComponentFileCreator parentClassCreator, string keyWord,
            AutoElementData element)
        {
            _classAppenderContainer.GetAppender(keyWord).AppendToClass(parentClassCreator, element);
        }

        private IEnumerable<ComponentGeneratorOutput> GetFileCreatorsOutput(string selector,
            IEnumerable<AutoElementData> children, string keyWord, ElementSelectorData[] childrenData)
        {
            IEnumerable<ComponentGeneratorOutput> outputs = new List<ComponentGeneratorOutput>();
            IComponentFileCreator parent = _fileCreatorContainer.GetFileCreator(keyWord);
            foreach (AutoElementData child in children)
            {
                outputs = outputs.Union(CreateCsOutput(child.Selector, child, parent), new ComponentOutputComparer());
            }
            if (parent != null)
            {
                ComponentGeneratorOutput parentOutput = parent.GenerateComponentClass(selector, childrenData);
                outputs = outputs.Union(new[] { parentOutput }, new ComponentOutputComparer());
            }
            return outputs;
        }

        private IEnumerable<ElementSelectorData> TransformFileCreatorsToAddinsLike(
            IEnumerable<ElementSelectorData> childrenData)
        {
            foreach (var child in childrenData)
            {
                IComponentFileCreator fileCreator = _fileCreatorContainer.GetFileCreator(child.Type);
                if (fileCreator != null)
                {
                    _addinContainer.AddAddin(fileCreator.MakeAddin(child.FullSelector));
                    child.Type = child.Name;
                }
                yield return child;
            }
        }

        private bool FilterNonInlineChidren(AutoElementData childData)
        {
            string keyWord = SelectorUtils.GetKeyWordFromSelector(childData.Selector);

            return !_classAppenderContainer.ContainsAppender(keyWord);
        }
    }
}