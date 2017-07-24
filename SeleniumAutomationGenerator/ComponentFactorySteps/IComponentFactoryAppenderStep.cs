using Core;
using Core.Models;

namespace SeleniumAutomationGenerator
{
    public interface IComponentFactoryAppenderStep : IComponentFactoryStep
    {
        void InvokeStep(AutoElementData rootElement, IComponentFileCreator parent);
    }
}