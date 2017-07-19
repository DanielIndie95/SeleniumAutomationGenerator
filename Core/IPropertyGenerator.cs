using System.Collections.Generic;
using Core.Models;

namespace Core
{
    public interface IPropertyGenerator
    {
        Property CreateProperty(IComponentAddin type , string propName, string selector);
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="propName"></param>
        /// <param name="selector"></param>
        /// <returns>key-private web element , value - main element</returns>
        KeyValuePair<Property,Property> CreatePropertyWithPrivateWebElement(IComponentAddin type , string propName, string selector);
        
        string GetPropertyName(string type, string propName);
        
        
    }   
}