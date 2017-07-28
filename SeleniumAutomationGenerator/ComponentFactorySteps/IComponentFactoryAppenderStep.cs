using Core;
using Core.Models;

namespace SeleniumAutomationGenerator.ComponentFactorySteps
{
    public interface IComponentFactoryAppenderStep : IComponentFactoryStep
    {
        void InvokeStep(AutoElementData rootElement, IComponentFileCreator parent);
    }
}