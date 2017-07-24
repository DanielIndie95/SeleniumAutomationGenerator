using Core;
using Core.Models;
using Core.Utils;
using System.Collections.Generic;
using System.Linq;

namespace SeleniumAutomationGenerator.ComponentFactorySteps
{
    public class FileCreatorFactoryStep : ComponentFactoryCreatorStep
    {
        readonly IFileCreatorContainer _container;
        private readonly IAddinContainer _addinsContainer;

        public FileCreatorFactoryStep(IFileCreatorContainer container, IClassAppenderContainer appendersContainer, IAddinContainer addinsContainer) : base(appendersContainer)
        {
            _container = container;
            _addinsContainer = addinsContainer;
        }

        public override bool ShouldInvokeStep(AutoElementData rootElement, IComponentFileCreator parent)
        {
            string type = SelectorUtils.GetKeyWordFromSelector(rootElement.Selector);
            return _container.GetFileCreator(type) != null;
        }

        public override IEnumerable<ComponentGeneratorOutput> InvokeStep(AutoElementData rootElement, IComponentFileCreator parent, IComponentFactoryCreatorStep next = null)
        {
            string selector = rootElement.Selector;
            AutoElementData[] filteredChildren = rootElement.InnerChildrens
                .Where(elm => !IsAppenderElement(elm))
                .ToArray();
            string keyWord = SelectorUtils.GetKeyWordFromSelector(selector);
            return GenerateAppendersOutputs(keyWord, rootElement.InnerChildrens, next)
                            .Union(GenerateFileCreatorOutputs(selector, filteredChildren, keyWord, next), new ComponentOutputComparer());
        }

        private IEnumerable<ComponentGeneratorOutput> GenerateAppendersOutputs(string keyword, IEnumerable<AutoElementData> elements, IComponentFactoryCreatorStep next)
        {
            IComponentFileCreator parent = _container.GetFileCreator(keyword);
            IEnumerable<AutoElementData> appendersElements = elements
                            .Where(IsAppenderElement);
            return GenerateClassesForElements(appendersElements, parent, next);
        }

        private IEnumerable<ComponentGeneratorOutput> GenerateFileCreatorOutputs(string selector,
            AutoElementData[] children, string keyWord, IComponentFactoryCreatorStep next)
        {
            ElementSelectorData[] elements = children
                            .Select(ConversionsUtils.ConvertToElementSelectorData)
                            .Select(TransformFileCreatorToAddinsLike)
                            .ToArray();

            IComponentFileCreator current = _container.GetFileCreator(keyWord);
            IEnumerable<ComponentGeneratorOutput> outputs = GenerateClassesForElements(children, current, next);

            ComponentGeneratorOutput parentOutput = current.GenerateComponentClass(selector, elements);
            outputs = outputs.Union(new[] { parentOutput }, new ComponentOutputComparer());

            return outputs;
        }

        private ElementSelectorData TransformFileCreatorToAddinsLike(ElementSelectorData child)
        {
            if (!IsAddinElement(child))
            {
                IComponentFileCreator fileCreator = _container.GetFileCreator(child.Type);
                if (fileCreator != null)
                {
                    _addinsContainer.AddAddin(fileCreator.MakeAddin(child.FullSelector));
                    child.Type = child.Name;
                }
            }
            return child;
        }

        private bool IsAddinElement(ElementSelectorData childData)
        {
            return _addinsContainer.ContainsAddin(childData.Type);
        }

    }
}
