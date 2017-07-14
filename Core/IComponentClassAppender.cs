using SeleniumAutomationGenerator.Models;

namespace SeleniumAutomationGenerator.Generator
{
    public interface IComponentClassAppender
    {
        void AppendToClass(IComponentFileCreator parentClass, string selector, ElementSelectorData[] elements);
    }
}
