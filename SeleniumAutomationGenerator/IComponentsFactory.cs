using System.Collections.Generic;
using Core;
using Core.Models;

namespace SeleniumAutomationGenerator
{
    public interface IComponentsFactory
    {
        IEnumerable<ComponentGeneratorOutput> CreateCsOutput(string body);
    }
}