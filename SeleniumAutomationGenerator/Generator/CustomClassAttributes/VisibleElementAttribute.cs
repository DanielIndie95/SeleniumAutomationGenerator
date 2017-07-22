using Core;
using Core.Utils;
using Core.Models;
using System.Collections.Generic;

namespace SeleniumAutomationGenerator.Generator.CustomClassAttributes
{
    public class VisibleElementAttribute : IElementAttribute
    {
        private readonly IAddinContainer _addinContainer;
        public VisibleElementAttribute(IAddinContainer addinContainer)
        {
            _addinContainer = addinContainer;
        }

        public string Identifier => "auto-visibility";

        public void AppendToClass(IComponentFileCreator parentClass, AutoElementData appenderElement)
        {
            ElementSelectorData element = ConversionsUtils.ConvertToElementSelectorData(appenderElement);
            KeyValuePair<Property, Property> propertyWithPrivateWebElement =
                    parentClass.PropertyGenerator.CreatePropertyWithPrivateWebElement(
                        _addinContainer.GetAddin(element.Type) ?? DefaultAddin.Create(element.Type), element.Name,
                        element.FullSelector);
            string privateWebElement = propertyWithPrivateWebElement.Key.Name;
            parentClass.AddProperty(GetProperty(privateWebElement));            
        }

        private static string GetProperty(string webElementPropertyName)
        {
            string propertyName = TextUtils.UppercaseFirst(webElementPropertyName.Trim('_'));
            string property = $"public bool {propertyName}Visible => {webElementPropertyName}.Displayed;";
            return property;
        }
    }
}