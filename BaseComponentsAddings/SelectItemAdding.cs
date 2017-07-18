using System.Text;
using Core;

namespace BaseComponentsAddins
{
    public class SelectItemAdding : IComponentAddin
    {
        public string AddinKey => "select";

        public string Type => "SelectElement";

        public string[] RequiredUsings => new [] { "OpenQA.Selenium.Support.UI" };

        public bool IsPropertyModifierPublic => false;

        public bool IsArrayedAddin => false;

        public bool CtorContainsDriver => false;

        public string[] GenerateHelpers(string className, string propName, IPropertyGenerator generator)
        {
            StringBuilder builder = new StringBuilder();
            string propertyName = generator.GetPropertyName(Type, propName);
            string helper =  builder.AppendLine($"public {className} Select{propName}(string value)")
                .AppendLine("{")
                .AppendLine($"{propertyName}.SelectByValue(value)")
                .AppendLine("}")
                .ToString();
            return new[] { helper };

        }
    }
}
