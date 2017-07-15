﻿using SeleniumAutomationGenerator.Generator.ComponentsGenerators;
using SeleniumAutomationGenerator.Utils;
using System.Text;

namespace SeleniumAutomationGenerator.Generator
{
    public class BasicModelGenerator : BasicClassGenerator
    {
        private string _parentFieldName;
        public BasicModelGenerator(ComponentsContainer container, IPropertyGenerator propertyGenerator, string namespaceName, string parentElementName) : base(container, propertyGenerator, namespaceName)
        {
            _parentFieldName = parentElementName;
        }
        public override IComponentAddin MakeAddin(string selector)
        {
            string name = SelectorUtils.GetClassOrPropNameFromSelector(selector);
            return new FileCreatorAddin()
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
