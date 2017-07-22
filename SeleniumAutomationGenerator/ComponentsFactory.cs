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

        public ComponentsFactory(IFileCreatorContainer fileCreatorContainer, IClassAppenderContainer classAppenderContainer, IAddinContainer addinContainer, IElementAttributeContainer attributesContainer)
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
            HandleCustomAttributesBehavior(current, parentClassCreator);

            if (IsBasicElement(current))
                return new List<ComponentGeneratorOutput>();

            string keyWord = SelectorUtils.GetKeyWordFromSelector(selector);
            AutoElementData[] children = current.InnerChildrens as AutoElementData[] ?? current.InnerChildrens.ToArray();
            AutoElementData[] filteredChildren = children
                .Where(elm => !IsAppenderElement(elm))
                .ToArray();

            if (IsAppenderElement(current))
            {
                return GenerateAppenderOutputs(current, parentClassCreator, keyWord, filteredChildren);
            }

            return GenerateFileCreatorOutputs(selector, keyWord, children, filteredChildren);
        }

        private IEnumerable<ComponentGeneratorOutput> GenerateFileCreatorOutputs(string selector, string keyWord, AutoElementData[] children, AutoElementData[] filteredChildren)
        {
            return GenerateAppendersOutputs(keyWord, children)
                            .Concat(GenerateFileCreatorOutputs(selector, filteredChildren, keyWord));
        }

        private IEnumerable<ComponentGeneratorOutput> GenerateAppendersOutputs(string keyword, AutoElementData[] elements)
        {
            IComponentFileCreator parent = _fileCreatorContainer.GetFileCreator(keyword);
            IEnumerable<AutoElementData> enumerable = elements
                            .Where(IsAppenderElement);
            return GenerateClassForElements(enumerable, parent);
        }        

        private void HandleCustomAttributesBehavior(AutoElementData current, IComponentFileCreator parentClassCreator)
        {
            if (ContainsCustomAttributes(current) && parentClassCreator != null)
                RunAppendsOnParent(current, parentClassCreator);
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
            if (parentClassCreator != null)
                RunAppender(parentClassCreator, keyWord, current);
            return GenerateClassForElements(filteredChildren);
        }

        private IEnumerable<ComponentGeneratorOutput> GenerateClassForElements(IEnumerable<AutoElementData> filteredChildren, IComponentFileCreator parent = null)
        {
            IEnumerable<ComponentGeneratorOutput> outputs = new List<ComponentGeneratorOutput>();
            foreach (AutoElementData child in filteredChildren)
            {
                outputs = outputs.Concat(CreateCsOutput(child.Selector, child, parent));
            }

            return outputs;
        }

        private bool IsAppenderSuitable(IComponentFileCreator parentClassCreator, string keyWord)
        {
            return _classAppenderContainer.ContainsAppender(keyWord)
                && parentClassCreator != null;
        }

        private void RunAppender(IComponentFileCreator parentClassCreator, string keyWord,
            AutoElementData element)
        {
            _classAppenderContainer.GetAppender(keyWord).AppendToClass(parentClassCreator, element);
        }

        private IEnumerable<ComponentGeneratorOutput> GenerateFileCreatorOutputs(string selector,
            IEnumerable<AutoElementData> children, string keyWord)
        {
            ElementSelectorData[] elements = children
                            .Select(ConversionsUtils.ConvertToElementSelectorData)
                            .Select(TransformFileCreatorToAddinsLike)
                            .ToArray();

            IComponentFileCreator parent = _fileCreatorContainer.GetFileCreator(keyWord);
            IEnumerable<ComponentGeneratorOutput> outputs = GenerateClassForElements(children);

            if (parent != null)
            {
                ComponentGeneratorOutput parentOutput = parent.GenerateComponentClass(selector, elements);
                outputs = outputs.Union(new[] { parentOutput }, new ComponentOutputComparer());
            }
            return outputs;
        }

        private ElementSelectorData TransformFileCreatorToAddinsLike(ElementSelectorData child)
        {
            if (!IsAddinElement(child))
            {
                IComponentFileCreator fileCreator = _fileCreatorContainer.GetFileCreator(child.Type);
                if (fileCreator != null)
                {
                    _addinContainer.AddAddin(fileCreator.MakeAddin(child.FullSelector));
                    child.Type = child.Name;
                }
            }
            return child;
        }

        private bool IsAddinElement(ElementSelectorData childData)
        {
            return _addinContainer.ContainsAddin(childData.Type);
        }
        private bool IsAppenderElement(AutoElementData childData)
        {
            string keyWord = SelectorUtils.GetKeyWordFromSelector(childData.Selector);

            return _classAppenderContainer.ContainsAppender(keyWord);
        }
        private static bool IsBasicElement(AutoElementData element)
        {
            return !element.InnerChildrens.Any();
        }
    }
}