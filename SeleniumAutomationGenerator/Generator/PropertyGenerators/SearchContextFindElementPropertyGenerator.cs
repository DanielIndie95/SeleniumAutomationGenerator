using System;
using System.Collections.Generic;
using Core;
using Core.Utils;

namespace SeleniumAutomationGenerator.Generator.PropertyGenerators
{
    public abstract class SearchContextFindElementPropertyGenerator : IPropertyGenerator
    {
        protected Dictionary<string, Func<string, string>> SinglePropertyExceptions;
        protected Dictionary<string, Func<string, string>> ListPropertyExceptions;
        protected string DriverPropertyName;

        protected SearchContextFindElementPropertyGenerator(string driverPropertyName)
        {
            DriverPropertyName = driverPropertyName;
            SinglePropertyExceptions = new Dictionary<string, Func<string, string>>();
            ListPropertyExceptions = new Dictionary<string, Func<string, string>>();
            AddException(Consts.WEB_ELEMENT_CLASS_NAME, selector => FindElementString(selector, false));
            AddException("string", selector => $"{FindElementString(selector, false)}.Text");
            AddException("int", selector => $"int.Parse({FindElementString(selector, false)}.Text)");

            AddException("string", selector => $"{FindElementString(selector, true)}.Select(elm=> elm.Text)", true);
            AddException("int", selector => $"{FindElementString(selector, true)}.Select(elm=> int.Parse(elm.Text))", true);
        }

        /// <summary>
        /// </summary>
        /// <param name="type"></param>
        /// <param name="exceptionValueGenerator">the first string is the selector(class), the second is the returned value</param>
        /// <param name="asListPropertyException">the exception is for the list case</param>
        public void AddException(string type, Func<string, string> exceptionValueGenerator, bool asListPropertyException = false)
        {
            if (asListPropertyException)
                ListPropertyExceptions[type] = exceptionValueGenerator;
            else
                SinglePropertyExceptions[type] = exceptionValueGenerator;
        }

        public virtual string CreateProperty(IComponentAddin addin, string propName, string selector)
        {
            string declerationStatement = GetDeclarationStatement(addin, propName);
            string equalsStatement = GetEqualStatement(addin, selector);
            return $"{declerationStatement} => {equalsStatement};";
        }

        private string GetEqualStatement(IComponentAddin addin, string selector)
        {
            if (IsExceptionType(addin))
                return HandleEqualStatmentExcpetions(addin, selector);
            string selectStatement = FindElementString(selector, addin.IsArrayedAddin);
            string modelInitialize = GetModelInitialize(addin, selectStatement);
            return FormatEqualStatement(addin, selectStatement, modelInitialize);
        }

        private string FormatEqualStatement(IComponentAddin addin, string selectStatement, string modelInitialize)
        {
            return addin.IsArrayedAddin ? $"{selectStatement}.Select(elm=> {modelInitialize})"
                : modelInitialize;
        }

        private string GetModelInitialize(IComponentAddin addin, string selectStatement)
        {
            string secondArgument = addin.IsArrayedAddin ? "elm" : selectStatement;
            return addin.CtorContainsDriver ? GetEqualArguments(addin.Type, DriverPropertyName, secondArgument)
                : GetEqualArguments(addin.Type, secondArgument);
        }

        private string GetEqualArguments(string type, params string[] typeArguments)
        {
            string arguments = string.Join(",", typeArguments);
            return $"new { type }({arguments})";
        }

        private string GetDeclarationStatement(IComponentAddin addin, string propName)
        {
            string modifier = GetModifier(addin);
            string type = GetDeclearationType(addin);
            propName = GetPropertyName(addin, propName);
            return $"{modifier} {type} {propName}";
        }

        public string GetPropertyName(IComponentAddin addin, string propName)
        {
            propName = addin.Type == Consts.WEB_ELEMENT_CLASS_NAME ? propName + "Element" : propName;//looks more logical to me
            return propName;
        }

        protected abstract string FindElementString(string selector, bool asList);

        private bool IsExceptionType(IComponentAddin addin)
        {
            if (addin.IsArrayedAddin)
                return ListPropertyExceptions.ContainsKey(addin.Type);
            return SinglePropertyExceptions.ContainsKey(addin.Type);
        }
        private string HandleEqualStatmentExcpetions(IComponentAddin addin, string selector)
        {
            if (addin.IsArrayedAddin)
                return ListPropertyExceptions[addin.Type](selector);
            return SinglePropertyExceptions[addin.Type](selector);
        }

        private string GetModifier(IComponentAddin addin)
        {
            return addin.IsPropertyModifierPublic ? "public" : "protected";
        }

        private string GetDeclearationType(IComponentAddin addin)
        {
            return addin.IsArrayedAddin ? $"ReadOnlyList<{addin.Type}>" : addin.Type;
        }

    }
}
