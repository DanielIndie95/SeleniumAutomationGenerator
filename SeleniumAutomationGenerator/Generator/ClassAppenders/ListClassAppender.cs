using BaseComponentsAddings;
using SeleniumAutomationGenerator.Models;
using SeleniumAutomationGenerator.Utils;


namespace SeleniumAutomationGenerator.Generator.ClassAppenders
{
    public class ListClassAppender : IComponentClassAppender
    {
        public void AppendToClass(IComponentFileCreator parentClass, string selector, ElementSelectorData[] elements)
        {
            ListItemAddin addin = new ListItemAddin();
            string name = SelectorUtils.GetClassNameFromSelector(selector);
            parentClass.AddProperty(parentClass.PropertyGenerator.CreateProperty(addin, name, selector));
        }
    }
}
