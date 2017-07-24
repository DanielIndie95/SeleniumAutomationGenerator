using System;
using System.Collections.Generic;
using Core;
using Core.Models;
using Core.Utils;
using System.Linq;

namespace SeleniumAutomationGenerator.ComponentFactorySteps
{
    public class AppenderCreatorStep : ComponentFactoryCreatorStep
    {
        public AppenderCreatorStep(IClassAppenderContainer container) : base(container)
        {
        }

        public override IEnumerable<ComponentGeneratorOutput> InvokeStep(AutoElementData rootElement, IComponentFileCreator parent, IComponentFactoryCreatorStep next = null)
        {
            string keyWord = SelectorUtils.GetKeyWordFromSelector(rootElement.Selector);
            AutoElementData[] filteredChildren = rootElement.InnerChildrens
                .Where(elm => !IsAppenderElement(elm))
                .ToArray();

            if (parent != null)
                AppendersContainer.GetAppender(keyWord).AppendToClass(parent, rootElement);
            return GenerateClassesForElements(filteredChildren, null, next);
        }

        public override bool ShouldInvokeStep(AutoElementData rootElement, IComponentFileCreator parent)
        {
            return IsAppenderElement(rootElement);
        }
    }
}
