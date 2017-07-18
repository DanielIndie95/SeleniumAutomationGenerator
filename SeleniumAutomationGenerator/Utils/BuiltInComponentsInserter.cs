namespace Core.Utils
{
    public static class BuiltInComponentsInserter
    {
        private void InsertBuiltInComponents()
        {
            ComponentsContainer.Instance.AddAddin(new InputAddin());
            ComponentsContainer.Instance.AddAddin(new LabelAddin());
            ComponentsContainer.Instance.AddAddin(new ButtonAddin());
            ComponentsContainer.Instance.AddAddin(new ListItemAddin());
            ComponentsContainer.Instance.AddAddin(new SelectItemAdding());
            ComponentsContainer.Instance.AddCustomAttribute(new VisibleElementAttribute());
        }
    }
}