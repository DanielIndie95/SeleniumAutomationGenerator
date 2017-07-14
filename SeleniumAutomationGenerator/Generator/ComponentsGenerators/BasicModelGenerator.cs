using System.Text;

namespace SeleniumAutomationGenerator.Generator
{
    public class BasicModelGenerator : BasicClassGenerator
    {
        private string _parentFieldName;
        public BasicModelGenerator(IAddinsContainer container, IPropertyGenerator propertyGenerator, string namespaceName, string parentElementName) : base(container, propertyGenerator, namespaceName)
        {
            _parentFieldName = parentElementName;
        }

        protected override string CreateCtor(string className)
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine($"internal {className}({Consts.WEB_ELEMENT_CLASS_NAME} parentElement)");
            builder.AppendLine("{");
            builder.AppendLine($"{_parentFieldName} = parentElement");
            builder.AppendLine("}");
            return builder.ToString();
        }

        protected override string[] GetFields()
        {
            string parentElementField = BasicClassBuilder.CreateField(Consts.WEB_ELEMENT_CLASS_NAME, _parentFieldName);
            return new string[] { parentElementField };
        }
    }
}
