namespace SeleniumAutomationGenerator.Generator.ComponentsGenerators
{
    public class DefaultAddin : IComponentAddin
    {
        public string AddinKey => Type;

        public string Type { get; set; }

        public string[] RequiredUsings { get; set; } = new string[] { };

        public bool IsPropertyModifierPublic { get; set; } = true;

        public bool IsArrayedAddin { get; set; } = false;

        public bool CtorContainsDriver { get; set; } = true;

        public string[] GenerateHelpers(string className, string selector, IPropertyGenerator generator)
        {
            return new string[] { };
        }

        public static DefaultAddin Create(string type)
        {
            return new DefaultAddin()
            {
                Type = type
            };
        }
    }
}
