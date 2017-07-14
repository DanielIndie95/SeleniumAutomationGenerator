using SeleniumAutomationGenerator;
using System;
using System.Text;

namespace BaseComponentsAddings
{
    public class SelectItemAdding : IComponentAddin
    {
        public string AddinKey => "select";

        public string Type => "SelectElement";

        public string[] RequiredUsings => new [] { "OpenQA.Selenium.Support.UI" };

        public bool IsPropertyModifierPublic => false;

        public bool IsArrayedAddin => false;

        public bool CtorContainsDriver => false;

        public string[] GenerateHelpers(string className, string propName)
        {
            StringBuilder builder = new StringBuilder();
            string helper =  builder.AppendLine($"public {className} Select{propName}(string value)")
                .AppendLine("{")
                .AppendLine($"{propName}")
                .AppendLine("}")
                .ToString();
            return new[] { helper };

        }
    }
}
