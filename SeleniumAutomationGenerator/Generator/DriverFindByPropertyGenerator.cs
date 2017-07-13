namespace SeleniumAutomationGenerator
{
    public class DriverFindByPropertyGenerator : IPropertyGenerator
    {
        public string CreateNode(IComponentAddin addin, string propName, string selector)
        {
            string modifier = addin.IsPropertyModifierPublic ? "public" : "protected";
            if (addin.Type == Consts.WEB_DRIVER_CLASS_NAME)
                return $"{modifier} {addin.Type} {propName} => Driver.FindElement({selector})";
            return $"{modifier} {addin.Type} {propName} => new {addin.Type}(Driver,Driver.FindElement({selector}))";
        }

        public string CreateNodeAsList(string type, string propName, string selector)
        {
            return $"public ReadOnlyList<{type}> {propName} => Driver.FindElements({selector})";
        }
    }
}
