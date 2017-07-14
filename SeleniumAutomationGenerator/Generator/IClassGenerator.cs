using SeleniumAutomationGenerator.Models;

namespace SeleniumAutomationGenerator.Generator
{
    public interface IClassGenerator
    {
        string GenerateComponentClass(string className, ElementSelectorData[] elements);
    }
}
