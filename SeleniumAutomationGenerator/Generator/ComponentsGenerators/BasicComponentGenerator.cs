﻿using System.Text;
using Core;
using Core.Utils;
using SeleniumAutomationGenerator.Generator.Builders;

namespace SeleniumAutomationGenerator.Generator.ComponentsGenerators
{
    public class BasicComponentGenerator : BasicClassGenerator
    {
        private readonly string _parentElementFieldName;
        public BasicComponentGenerator(IPropertyGenerator propertyGenerator, string namespaceName, string parentElementFieldName) : base(propertyGenerator, namespaceName)
        {
            _parentElementFieldName = parentElementFieldName;
        }

        protected override string CreateCtor(string className)
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine($"internal {className}({Consts.WEB_DRIVER_CLASS_NAME} driver,{Consts.WEB_ELEMENT_CLASS_NAME} parentElement)");
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
