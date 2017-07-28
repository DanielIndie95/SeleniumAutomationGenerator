using Core;
using Core.Models;

namespace SeleniumAutomationGenerator.ComponentFactorySteps
{
    public interface IComponentFactoryStep
    {
        bool ShouldInvokeStep(AutoElementData rootElement, IComponentFileCreator parent);
    }
}