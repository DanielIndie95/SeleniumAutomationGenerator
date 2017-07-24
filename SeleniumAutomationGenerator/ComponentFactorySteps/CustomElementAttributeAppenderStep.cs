using Core;
using Core.Models;
using System.Linq;
using MoreLinq;

namespace SeleniumAutomationGenerator.ComponentFactorySteps
{
    public class CustomElementAttributeAppenderStep : IComponentFactoryAppenderStep
    {
        readonly IElementAttributeContainer _container;
        public CustomElementAttributeAppenderStep(IElementAttributeContainer container)
        {
            _container = container;
        }

        public void InvokeStep(AutoElementData rootElement, IComponentFileCreator parent)
        {
            rootElement.AutoAttributes
               .Where(_container.ContainsCustomAttribute)
               .Select(_container.GetElementAttribute)
               .ForEach(att => att.AppendToClass(parent, rootElement));
        }

        public bool ShouldInvokeStep(AutoElementData rootElement, IComponentFileCreator parent)
        {
            return rootElement.AutoAttributes
                .Any(_container.ContainsCustomAttribute) && parent != null;
        }
    }
}
