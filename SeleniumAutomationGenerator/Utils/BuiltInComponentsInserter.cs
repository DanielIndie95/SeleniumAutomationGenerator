using BaseComponentsAddins;
using Core;
using SeleniumAutomationGenerator.Generator.CustomClassAttributes;

namespace SeleniumAutomationGenerator.Utils
{
    public static class BuiltInComponentsInserter
    {
        public static void InsertBuiltInComponents()
        {
            ComponentsContainer.Instance.AddAddin(new InputAddin());
            ComponentsContainer.Instance.AddAddin(new LabelAddin());
            ComponentsContainer.Instance.AddAddin(new ButtonAddin());
            ComponentsContainer.Instance.AddAddin(new ListItemAddin());
            ComponentsContainer.Instance.AddAddin(new SelectItemAdding());
            ComponentsContainer.Instance.AddCustomAttribute(new VisibleElementAttribute());
            ComponentsContainer.Instance.AddCustomAttribute(new WaitUntilDisplayedElementAttribute());
        }
    }
}