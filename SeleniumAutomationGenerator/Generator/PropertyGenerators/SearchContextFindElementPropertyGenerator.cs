using System;
using System.Collections.Generic;

namespace SeleniumAutomationGenerator.Generator
{
    public abstract class SearchContextFindElementPropertyGenerator : IPropertyGenerator
    {
        public Dictionary<string, Func<string, string>> _singlePropertyExceptions;
        public Dictionary<string, Func<string, string>> _listPropertyExceptions;
        protected string DriverPropertyName;
        public SearchContextFindElementPropertyGenerator(string driverPropertyName)
        {
            DriverPropertyName = driverPropertyName;
            _singlePropertyExceptions = new Dictionary<string, Func<string, string>>();
            _listPropertyExceptions = new Dictionary<string, Func<string, string>>();
            AddException(Consts.WEB_ELEMENT_CLASS_NAME, (selector) => FindElementString(selector, false));
            AddException("string", (selector) => $"{FindElementString(selector, false)}.Text");
            AddException("int", (selector) => $"int.Parse({FindElementString(selector, false)}.Text)");

            AddException("string", (selector) => $"{FindElementString(selector, false)}.Select(elm=> elm.Text)", true);
            AddException("int", (selector) => $"{FindElementString(selector, false)}.Select(elm=> int.Parse(elm.Text))", true);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="exceptionValueGenerator">the first string is the selector(class), the second is the returned value</param>
        public void AddException(string type, Func<string, string> exceptionValueGenerator, bool asListPropertyException = false)
        {
            if (asListPropertyException)
                _listPropertyExceptions[type] = exceptionValueGenerator;
            else
                _singlePropertyExceptions[type] = exceptionValueGenerator;
        }

        public virtual string CreateProperty(IComponentAddin addin, string propName, string selector)
        {
            if (addin.IsArrayedAddin)
                return CreateListTypeProperty(addin, propName, selector);
            return CreateSingleTypeProperty(addin, propName, selector);
        }

        private string CreateSingleTypeProperty(IComponentAddin addin, string propName, string selector)
        {
            string modifier = GetModifier(addin);
            propName = GetPropertyName(addin, propName);
            if (IsExceptionType(addin.Type))
                return $"{modifier} {addin.Type} {propName} => {HandleExcpetions(addin.Type, selector)};";

            return $"{modifier} {addin.Type} {propName} => new {addin.Type}({DriverPropertyName},{FindElementString(selector, false)});";
        }

        public string GetPropertyName(IComponentAddin addin, string propName)
        {
            propName = addin.Type == Consts.WEB_ELEMENT_CLASS_NAME ? propName + "Element" : propName;//looks more logical to me
            return propName;
        }

        public virtual string CreateListTypeProperty(IComponentAddin addin, string propName, string selector)
        {
            string modifier = GetModifier(addin);
            return $"{modifier} ReadOnlyList<{addin.Type}> {propName} => {FindElementString(selector, true)};";
        }

        protected abstract string FindElementString(string selector, bool asList);

        private bool IsExceptionType(string type)
        {
            return _singlePropertyExceptions.ContainsKey(type);
        }
        private string HandleExcpetions(string type, string selector, bool asList = false)
        {
            if (asList)
                return _listPropertyExceptions[type](selector);
            return _singlePropertyExceptions[type](selector);
        }

        private string GetModifier(IComponentAddin addin)
        {
            return addin.IsPropertyModifierPublic ? "public" : "protected";
        }
    }
}
