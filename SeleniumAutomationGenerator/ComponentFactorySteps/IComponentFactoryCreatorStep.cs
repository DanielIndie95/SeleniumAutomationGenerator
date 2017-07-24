using Core;
using Core.Models;
using System.Collections.Generic;
using System;

namespace SeleniumAutomationGenerator
{
    public interface IComponentFactoryCreatorStep : IComponentFactoryStep
    {
        IEnumerable<ComponentGeneratorOutput> InvokeStep(AutoElementData rootElement, IComponentFileCreator parent, IComponentFactoryCreatorStep next = null);
    }
}