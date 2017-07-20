using System;
using System.Collections.Generic;
using Core;
using Core.Models;
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
            AddException(Consts.WEB_ELEMENT_CLASS_NAME, selectElement => selectElement);
            AddException("string", selectElement => $"{selectElement}.Text");
            AddException("int", selectElement => $"int.Parse({selectElement}.Text)");

            AddException("string", selectElement => $"{selectElement}.Select(elm=> elm.Text)", true);
            AddException("int", selectElement => $"{selectElement}.Select(elm=> int.Parse(elm.Text))",
                true);
        }

        /// <summary>
        /// </summary>
        /// <param name="type"></param>
        /// <param name="exceptionValueGenerator">the first string is the selector(class), the second is the returned value</param>
        /// <param name="asListPropertyException">the exception is for the list case</param>
        public void AddException(string type, Func<string, string> exceptionValueGenerator,
            bool asListPropertyException = false)
        {
            if (asListPropertyException)
                ListPropertyExceptions[type] = exceptionValueGenerator;
            else
                SinglePropertyExceptions[type] = exceptionValueGenerator;
        }

        public virtual Property CreateProperty(IComponentAddin addin, string propName, string selector)
        {
            string modifier = GetModifier(addin);
            return CreateProperty(modifier, addin.Type, addin.IsArrayedAddin, addin.CtorContainsDriver, propName,
                selector);
        }


        public KeyValuePair<Property, Property> CreatePropertyWithPrivateWebElement(IComponentAddin addin,
            string propName,
            string selector)
        {
            string modifier = GetModifier(addin);

            var privateProperty = CreateProperty("private", Consts.WEB_ELEMENT_CLASS_NAME, addin.IsArrayedAddin, false,
                "_" + propName, selector); //_ string is for private naming

            string declerationStatement = GetDeclarationStatement(modifier, addin.Type, propName);
            string equalsStatement = GetEqualStatement(addin.Type, addin.IsArrayedAddin, addin.CtorContainsDriver,
                selector, privateProperty.Name);
            var mainProperty = new Property
            {
                Statement = $"{declerationStatement} => {equalsStatement};",
                Name = propName
            };
            return new KeyValuePair<Property, Property>(privateProperty, mainProperty);
        }

        public string GetPropertyName(string type, string propName)
        {
            propName = type == Consts.WEB_ELEMENT_CLASS_NAME
                ? propName + "Element"
                : propName; //looks more logical to me
            return propName;
        }

        private Property CreateProperty(string modifier, string type, bool isArray, bool containsDriver,
            string propName, string selector, string selectElement = null)
        {
            string propertyName = GetPropertyName(type, propName);
            string declarationType = GetDeclearationType(type, isArray);
            string declerationStatement = GetDeclarationStatement(modifier, declarationType, propName);
            string equalsStatement = GetEqualStatement(type, isArray, containsDriver, selector,selectElement);
            return new Property
            {
                Statement = $"{declerationStatement} => {equalsStatement};",
                Name = propertyName
            };
        }

        private string GetEqualStatement(string type, bool isArray, bool containsDriver, string selector,
            string selectStatement = null)
        {
            selectStatement = selectStatement ?? FindElementString(selector, isArray);
            if (IsExceptionType(isArray, type))
                return HandleEqualStatmentExcpetions(isArray, type, selectStatement);
            string modelInitialize = GetModelInitialize(isArray, containsDriver, type, selectStatement);
            return FormatEqualStatement(isArray, selectStatement, modelInitialize);
        }

        private string GetModelInitialize(bool isArray, bool containsDriver, string type, string selectStatement)
        {
            string secondArgument = isArray ? "elm" : selectStatement;
            return containsDriver
                ? GetEqualArguments(type, DriverPropertyName, secondArgument)
                : GetEqualArguments(type, secondArgument);
        }

        protected abstract string FindElementString(string selector, bool asList);

        private bool IsExceptionType(bool isArray, string type)
        {
            return isArray
                ? ListPropertyExceptions.ContainsKey(type)
                : SinglePropertyExceptions.ContainsKey(type);
        }

        private string GetDeclarationStatement(string modifier, string type, string propName)
        {
            propName = GetPropertyName(type, propName);
            return $"{modifier} {type} {propName}";
        }

        private string HandleEqualStatmentExcpetions(bool isArray, string type, string selector)
        {
            return isArray ? ListPropertyExceptions[type](selector) : SinglePropertyExceptions[type](selector);
        }

        private static string GetModifier(IComponentAddin addin)
        {
            return addin.IsPropertyModifierPublic ? "public" : "protected";
        }

        private static string GetDeclearationType(string type, bool isArray)
        {
            return isArray ? $"ReadOnlyList<{type}>" : type;
        }


        private static string GetEqualArguments(string type, params string[] typeArguments)
        {
            string arguments = string.Join(",", typeArguments);
            return $"new {type}({arguments})";
        }

        private static string FormatEqualStatement(bool isArray, string selectStatement, string modelInitialize)
        {
            return isArray
                ? $"{selectStatement}.Select(elm=> {modelInitialize})"
                : modelInitialize;
        }
    }
}