using System.Collections.Generic;
using System.Linq;
using Core;
using Core.Models;
using Core.Utils;

namespace SeleniumAutomationGenerator.ComponentFactorySteps
{
    public abstract class ComponentFactoryCreatorStep : IComponentFactoryCreatorStep
    {
        public IClassAppenderContainer AppendersContainer;

        protected ComponentFactoryCreatorStep(IClassAppenderContainer container)
        {
            AppendersContainer = container;
        }

        public abstract IEnumerable<ComponentGeneratorOutput> InvokeStep(AutoElementData rootElement, IComponentFileCreator parent, IComponentFactoryCreatorStep next = null);

        public abstract bool ShouldInvokeStep(AutoElementData rootElement, IComponentFileCreator parent);

        protected IEnumerable<ComponentGeneratorOutput> GenerateClassesForElements(IEnumerable<AutoElementData> children, IComponentFileCreator parent, IComponentFactoryCreatorStep next)
        {
            IEnumerable<ComponentGeneratorOutput> outputs = new List<ComponentGeneratorOutput>();
            foreach (AutoElementData child in children)
            {
                if (next?.ShouldInvokeStep(child, parent) ?? false)
                {
                    outputs = outputs.Union(next.InvokeStep(child, parent), new ComponentOutputComparer());
                }
            }

            return outputs;
        }
        protected bool IsAppenderElement(AutoElementData childData)
        {
            string keyWord = SelectorUtils.GetKeyWordFromSelector(childData.Selector);

            return AppendersContainer.ContainsAppender(keyWord);
        }
    }
}