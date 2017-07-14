using SeleniumAutomationGenerator.Models;

namespace SeleniumAutomationGenerator.Generator
{
    public interface IComponentFileCreator
    {
        ComponentGeneratorOutput GenerateComponentClass(string selector, ElementSelectorData[] elements);
        void AddExceptionPropertyType(string type);
        void AddProperty(string property);
    }
}
