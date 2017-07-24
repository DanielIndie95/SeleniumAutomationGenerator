using Core;
using Core.Models;

namespace SeleniumAutomationGenerator
{
    public interface IComponentFactoryStep
    {
        bool ShouldInvokeStep(AutoElementData rootElement, IComponentFileCreator parent);
    }
}