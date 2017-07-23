using System.Collections.Generic;
using System.Linq;
using Core;
using Core.Models;
using Core.Utils;
using SeleniumAutomationGenerator.Utils;
using MoreLinq;

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
            return GenerateClassesForElements(children);
        }

        private IEnumerable<ComponentGeneratorOutput> CreateCsOutput(string selector, AutoElementData current,
            IComponentFileCreator parentClassCreator = null)
        {
            RunCustomAttributesAppends(current, parentClassCreator);

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

        private IEnumerable<ComponentGeneratorOutput> GenerateFileCreatorOutputs(string selector, string keyWord, IEnumerable<AutoElementData> children, AutoElementData[] filteredChildren)
        {
            return GenerateAppendersOutputs(keyWord, children)
                            .Union(GenerateFileCreatorOutputs(selector, filteredChildren, keyWord), new ComponentOutputComparer());
        }

        private IEnumerable<ComponentGeneratorOutput> GenerateAppendersOutputs(string keyword, IEnumerable<AutoElementData> elements)
        {
            IComponentFileCreator parent = _fileCreatorContainer.GetFileCreator(keyword);
            IEnumerable<AutoElementData> appendersElements = elements
                            .Where(IsAppenderElement);
            return GenerateClassesForElements(appendersElements, parent);
        }

        private void RunCustomAttributesAppends(AutoElementData current, IComponentFileCreator parentClassCreator)
        {
            current.AutoAttributes
                .Where(_attributesContainer.ContainsCustomAttribute)
                .Select(_attributesContainer.GetElementAttribute)
                .ForEach(att => RunAppender(parentClassCreator, att, current));
        }

        private IEnumerable<ComponentGeneratorOutput> GenerateAppenderOutputs(AutoElementData current,
            IComponentFileCreator parentClassCreator, string keyWord, IEnumerable<AutoElementData> filteredChildren)
        {
            RunAppender(parentClassCreator, _classAppenderContainer.GetAppender(keyWord), current);
            return GenerateClassesForElements(filteredChildren);
        }

        private IEnumerable<ComponentGeneratorOutput> GenerateClassesForElements(IEnumerable<AutoElementData> children, IComponentFileCreator parent = null)
        {
            IEnumerable<ComponentGeneratorOutput> outputs = new List<ComponentGeneratorOutput>();
            foreach (AutoElementData child in children)
            {
                outputs = outputs.Union(CreateCsOutput(child.Selector, child, parent), new ComponentOutputComparer());
            }

            return outputs;
        }

        private void RunAppender(IComponentFileCreator parentClassCreator, IComponentClassAppender appender,
            AutoElementData element)
        {
            if (parentClassCreator != null)
                appender.AppendToClass(parentClassCreator, element);
        }

        private IEnumerable<ComponentGeneratorOutput> GenerateFileCreatorOutputs(string selector,
            AutoElementData[] children, string keyWord)
        {
            ElementSelectorData[] elements = children
                            .Select(ConversionsUtils.ConvertToElementSelectorData)
                            .Select(TransformFileCreatorToAddinsLike)
                            .ToArray();

            IComponentFileCreator parent = _fileCreatorContainer.GetFileCreator(keyWord);
            IEnumerable<ComponentGeneratorOutput> outputs = GenerateClassesForElements(children, parent);

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