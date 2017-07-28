using System.Collections.Generic;
using Core;
using Core.Models;

namespace SeleniumAutomationGenerator.ComponentFactorySteps
{
    public interface IComponentFactoryCreatorStep : IComponentFactoryStep
    {
        IEnumerable<ComponentGeneratorOutput> InvokeStep(AutoElementData rootElement, IComponentFileCreator parent, IComponentFactoryCreatorStep next = null);
    }
}