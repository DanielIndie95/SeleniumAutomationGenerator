using System.Text;

namespace SeleniumAutomationGenerator.Generator
{
    public class BasicPageGenerator : BasicClassGenerator
    {        
        public BasicPageGenerator(IAddinsContainer container, IPropertyGenerator propertyGenerator, string namespaceName) : base(container , propertyGenerator, namespaceName)
        {

        }       

        protected override string CreateCtor(string className)
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine($"internal {className}({Consts.WEB_DRIVER_CLASS_NAME} driver)");
            builder.AppendLine("{");
            builder.AppendLine("}");
            return builder.ToString();
        }       
    }
}
