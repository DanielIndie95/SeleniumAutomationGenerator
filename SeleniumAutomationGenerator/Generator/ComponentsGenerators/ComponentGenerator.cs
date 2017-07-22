using System.Text;
using Core;
using Core.Utils;
using SeleniumAutomationGenerator.Builders;

namespace SeleniumAutomationGenerator.Generator.ComponentsGenerators
{
    public class ComponentGenerator : ClassGenerator
    {
        private readonly string _parentElementFieldName;
        public ComponentGenerator(IClassBuilder builder, IPropertyGenerator propertyGenerator, IAddinContainer container, string namespaceName, string parentElementFieldName) : base(builder, propertyGenerator,container, namespaceName)
        {
            _parentElementFieldName = parentElementFieldName;
        }

        protected override string CreateCtor(string className)
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine($"internal {className}({Consts.WEB_DRIVER_CLASS_NAME} driver,{Consts.WEB_ELEMENT_CLASS_NAME} parentElement):base(driver)");
            builder.AppendLine("{");
            builder.AppendLine($"{_parentElementFieldName} = parentElement;");
            builder.Append(CtorBulk);
            builder.AppendLine("}");
            return builder.ToString();
        }

        protected override string[] GetFields()
        {
            string parentElementField = BasicClassBuilder.CreateField(Consts.WEB_ELEMENT_CLASS_NAME, _parentElementFieldName);
            return new[] { parentElementField };
        }
    }
}
