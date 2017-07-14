using System;
using System.Collections.Generic;

namespace SeleniumAutomationGenerator
{
    public class DriverFindElementPropertyGenerator : IPropertyGenerator
    {
        public Dictionary<string, Func<string, string>> _exceptions;
        public DriverFindElementPropertyGenerator()
        {
            _exceptions = new Dictionary<string, Func<string, string>>();
            AddException(Consts.WEB_DRIVER_CLASS_NAME, (selector) => FindElementString(selector, false));
            AddException("string", (selector) => $"{FindElementString(selector, false)}.Text");
            AddException("int", (selector) => $"int.Parse({FindElementString(selector, false)}.Text)");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="exceptionValueGenerator">the first string is the selector(class), the second is the returned value</param>
        public void AddException(string type , Func<string,string> exceptionValueGenerator)
        {
            _exceptions.Add(type, exceptionValueGenerator);
        }

        public string CreateNode(IComponentAddin addin, string propName, string selector)
        {
            string modifier = addin.IsPropertyModifierPublic ? "public" : "protected";
            if (IsExceptionType(addin.Type))
                return $"{modifier} {addin.Type} {propName} => {HandleExcpetions(addin.Type, selector)};";
            return $"{modifier} {addin.Type} {propName} => new {addin.Type}(Driver,{FindElementString(selector, false)});";
        }

        public string CreateNodeAsList(string type, string propName, string selector)
        {
            return $"public ReadOnlyList<{type}> {propName} => {FindElementString(selector, true)};";
        }

        private string FindElementString(string selector, bool asList)
        {
            string add = asList ? "s" : "";
            return $"Driver.FindElement{add}(By.ClassName(\"{selector}\"))";
        }

        private bool IsExceptionType(string type)
        {
            return _exceptions.ContainsKey(type);
        }
        private string HandleExcpetions(string type, string selector)
        {
            return _exceptions[type](selector);
        }
    }
}
