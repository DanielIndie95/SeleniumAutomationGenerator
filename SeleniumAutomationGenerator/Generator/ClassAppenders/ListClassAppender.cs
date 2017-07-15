using BaseComponentsAddings;
using SeleniumAutomationGenerator.Models;

namespace SeleniumAutomationGenerator.Generator.ClassAppenders
{
    public class ListClassAppender : IComponentClassAppender
    {
        public void AppendToClass(IComponentFileCreator parentClass, string selector, ElementSelectorData[] elements)
        {
            bool ctorContainsDriver;
            string type;
            if (elements.Length > 0)
            {
                type = elements[0].Type;
                selector = elements[0].FullSelector;
                ctorContainsDriver = ComponentsContainer.Instance.GetAddin(type)?.CtorContainsDriver ?? false;
            }
            else
            {
                type = "string";
                ctorContainsDriver = false;
            }
            string name = type + "List";
            ListItemAddin addin = new ListItemAddin()
            {
                Type = type,
                CtorContainsDriver = ctorContainsDriver
            };
            
            parentClass.AddProperty(parentClass.PropertyGenerator.CreateProperty(addin, name, selector));
        }
    }
}
