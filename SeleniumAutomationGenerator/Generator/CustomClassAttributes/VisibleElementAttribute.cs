using Core;
using Core.Utils;
using Core.Models;
using SeleniumAutomationGenerator.Generator.ComponentsGenerators;
using System.Collections.Generic;

namespace SeleniumAutomationGenerator.Generator.CustomClassAttributes
{
    public class VisibleElementAttribute : IElementAttribute
    {
        public string GetProperty(string webElementPropertyName)
        {
            string propertyName = TextUtils.UppercaseFirst(webElementPropertyName.Trim('_'));
            string property = $"public bool {propertyName}Visible => {webElementPropertyName}.Displayed;";
            return property;
        }        

        public void AppendToClass(IComponentFileCreator parentClass, AutoElementData appenderElement)
        {
            ElementSelectorData element = ConversionsUtils.ConvertToElementSelectorData(appenderElement);
            KeyValuePair<Property, Property> propertyWithPrivateWebElement =
                    parentClass.PropertyGenerator.CreatePropertyWithPrivateWebElement(
                        ComponentsContainer.Instance.GetAddin(element.Type) ?? DefaultAddin.Create(element.Type), element.Name,
                        element.FullSelector);
            string privateWebElement = propertyWithPrivateWebElement.Key.Name;
            parentClass.AddProperty(GetProperty(privateWebElement));            
        }

        public string Name => "auto-visibility";
    }
}