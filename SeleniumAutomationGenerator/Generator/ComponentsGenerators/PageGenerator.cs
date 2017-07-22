using System.Text;
using Core;
using Core.Utils;

namespace SeleniumAutomationGenerator.Generator.ComponentsGenerators
{
    public class PageGenerator : ClassGenerator
    {
        public PageGenerator(IClassBuilder builder, IPropertyGenerator propertyGenerator, IAddinContainer container, string namespaceName) : base(builder, propertyGenerator, container, namespaceName)
        {

        }

        protected override string CreateCtor(string className)
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine($"internal {className}({Consts.WEB_DRIVER_CLASS_NAME} driver):base(driver)");
            builder.AppendLine("{");
            builder.Append(CtorBulk);
            builder.AppendLine("}");
            return builder.ToString();
        }
    }
}
