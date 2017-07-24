using System.Collections.Generic;
using System.Linq;
using Core;
using Core.Models;

namespace SeleniumAutomationGenerator.ComponentFactorySteps
{
    internal class BasicElementCreatorStep : IComponentFactoryCreatorStep
    {
        public IEnumerable<ComponentGeneratorOutput> InvokeStep(AutoElementData rootElement, IComponentFileCreator parent, IComponentFactoryCreatorStep next = null)
        {
            return new List<ComponentGeneratorOutput>();
        }

        public bool ShouldInvokeStep(AutoElementData rootElement, IComponentFileCreator parent)
        {
            return IsBasicElement(rootElement);
        }

        private static bool IsBasicElement(AutoElementData element)
        {
            return !element.InnerChildrens.Any();
        }
    }
}
