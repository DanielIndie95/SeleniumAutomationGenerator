using Core;
using Core.Models;
using Core.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeleniumAutomationGenerator.ComponentFactorySteps
{
    public class FileCreatorFactoryStep : ComponentFactoryCreatorStep
    {
        IFileCreatorContainer _container;
        private IAddinContainer _addinsContainer;

        public FileCreatorFactoryStep(IFileCreatorContainer container, IClassAppenderContainer appendersContainer, IAddinContainer addinsContainer) : base(appendersContainer)
        {
            _container = container;
            _addinsContainer = addinsContainer;
        }

        private IEnumerable<ComponentGeneratorOutput> GenerateFileCreatorOutputs(string selector, IEnumerable<AutoElementData> children, AutoElementData[] filteredChildren, IComponentFactoryCreatorStep next)
        {
            string keyWord = SelectorUtils.GetKeyWordFromSelector(selector);
            return GenerateAppendersOutputs(keyWord, children, next)
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

            IComponentFileCreator parent = _container.GetFileCreator(keyWord);
            IEnumerable<ComponentGeneratorOutput> outputs = GenerateClassesForElements(children, parent, next);

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

        public override IEnumerable<ComponentGeneratorOutput> InvokeStep(AutoElementData rootElement, IComponentFileCreator parent, IComponentFactoryCreatorStep next = null)
        {
            string keyWord = SelectorUtils.GetKeyWordFromSelector(rootElement.Selector);
            AutoElementData[] children = rootElement.InnerChildrens as AutoElementData[] ?? rootElement.InnerChildrens.ToArray();
            AutoElementData[] filteredChildren = children
                .Where(elm => !IsAppenderElement(elm))
                .ToArray();
            return GenerateFileCreatorOutputs(rootElement.Selector, children, filteredChildren, next);
        }

        public override bool ShouldInvokeStep(AutoElementData rootElement, IComponentFileCreator parent)
        {
            return true;
        }
    }
}
