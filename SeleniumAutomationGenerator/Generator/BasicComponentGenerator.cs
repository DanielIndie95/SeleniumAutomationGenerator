using System.Text;

namespace SeleniumAutomationGenerator.Generator
{
    public class BasicComponentGenerator : BasicClassGenerator
    {
        public BasicComponentGenerator(IAddinsContainer container, IPropertyGenerator propertyGenerator, string namespaceName) : base(container, propertyGenerator, namespaceName)
        {
        }

        protected override string CreateCtor(string className)
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine($"internal {className}({Consts.WEB_DRIVER_CLASS_NAME} driver,{Consts.WEB_ELEMENT_CLASS_NAME} parentElement)");
            builder.AppendLine("{");
            builder.AppendLine("_parentElement = parentElement");
            builder.AppendLine("}");
            return builder.ToString();
        }

        protected override string[] GetFields()
        {
            string parentElementField = BasicClassBuilder.CreateField(Consts.WEB_ELEMENT_CLASS_NAME, "_parentElement");
            return new string[] { parentElementField };
        }
    }
}
