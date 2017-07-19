using System.Text;
using Core;
using Core.Utils;
using SeleniumAutomationGenerator.Generator.Builders;

namespace SeleniumAutomationGenerator.Generator.ComponentsGenerators
{
    public class BasicModelGenerator : BasicClassGenerator
    {
        private readonly string _parentFieldName;
        public BasicModelGenerator(IPropertyGenerator propertyGenerator, string namespaceName, string parentElementName) : base(propertyGenerator, namespaceName)
        {
            _parentFieldName = parentElementName;
        }
        public override IComponentAddin MakeAddin(string selector)
        {
            string name = SelectorUtils.GetClassOrPropNameFromSelector(selector);
            return new FileCreatorAddin
            {
                AddinKey = name,
                Type = name,
                CtorContainsDriver = false
            };
        }
        protected override string CreateCtor(string className)
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine($"internal {className}({Consts.WEB_ELEMENT_CLASS_NAME} parentElement)");
            builder.AppendLine("{");
            builder.AppendLine($"{_parentFieldName} = parentElement;");
            builder.Append(CtorBulk);
            builder.AppendLine("}");
            return builder.ToString();
        }

        protected override string[] GetFields()
        {
            string parentElementField = BasicClassBuilder.CreateField(Consts.WEB_ELEMENT_CLASS_NAME, _parentFieldName);
            return new[] { parentElementField };
        }
    }
}
