using System.Text;
using Core;
using Core.Utils;

namespace BaseComponentsAddins
{
    public class ButtonAddin : IComponentAddin
    {
        public string AddinKey => "button";

        public string Type => Consts.WEB_ELEMENT_CLASS_NAME;

        public string[] RequiredUsings => new string[] { };

        public bool IsPropertyModifierPublic => false;

        public bool IsArrayedAddin => false;

        public bool CtorContainsDriver => false;

        public string[] GenerateHelpers(string className, string selector, IPropertyGenerator generator)
        {
            string[] parts = selector.Split('-'); //auto-button-name-newComp
            string propName = parts[2];
            string propertyName = generator.GetPropertyName(Type, propName);
            string returnedClass = parts.Length == 4 ? parts[3] : className;
            string returnStatement = parts.Length == 4 ? "this" : $"new {returnedClass}({Consts.DRIVER_FIELD_NAME})";
            StringBuilder builder = new StringBuilder();
            string helper = builder.AppendLine($"public {returnedClass} Click{propName}()")
                .AppendLine("{")
                .AppendLine($"{propName}.Click();")
                .AppendLine($"return {returnStatement}")
                .AppendLine("}")
                .ToString();
            return new[] { helper };
        }
    }
}
