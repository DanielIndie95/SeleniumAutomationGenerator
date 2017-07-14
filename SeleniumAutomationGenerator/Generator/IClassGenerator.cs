using SeleniumAutomationGenerator.Models;

namespace SeleniumAutomationGenerator.Generator
{
    public interface IClassGenerator
    {
        ComponentGeneratorOutput GenerateComponentClass(string className, ElementSelectorData[] elements);
    }
}
