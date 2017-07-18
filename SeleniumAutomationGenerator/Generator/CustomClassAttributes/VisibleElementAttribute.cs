using System.Text;
using Core;
using Core.Utils;

namespace SeleniumAutomationGenerator.Generator.CustomClassAttributes
{
    public class VisibleElementAttribute : IElementAttribute
    {
        public string[] GetProperties(string webElementPropertyName)
        {
            string propertyName = TextUtils.UppercaseFirst(webElementPropertyName.Trim('_'));
            string property = $"public bool {propertyName}Visible => {webElementPropertyName}.Displayed;";
            return new string[] {property};
        }

        public string[] GetMethods(string webElementPropertyName)
        {
            return new string[0];
        }

        public string Name => "auto-visibility";
    }
}