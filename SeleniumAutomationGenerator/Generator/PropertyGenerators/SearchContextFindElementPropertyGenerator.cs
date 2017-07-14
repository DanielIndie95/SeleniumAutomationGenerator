﻿using System;
using System.Collections.Generic;

namespace SeleniumAutomationGenerator.Generator
{
    public abstract class SearchContextFindElementPropertyGenerator : IPropertyGenerator
    {
        public Dictionary<string, Func<string, string>> _exceptions;
        protected string DriverPropertyName;
        public SearchContextFindElementPropertyGenerator(string driverPropertyName)
        {
            DriverPropertyName = driverPropertyName;
            _exceptions = new Dictionary<string, Func<string, string>>();
            AddException(Consts.WEB_ELEMENT_CLASS_NAME, (selector) => FindElementString(selector, false));
            AddException("string", (selector) => $"{FindElementString(selector, false)}.Text");
            AddException("int", (selector) => $"int.Parse({FindElementString(selector, false)}.Text)");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="exceptionValueGenerator">the first string is the selector(class), the second is the returned value</param>
        public void AddException(string type, Func<string, string> exceptionValueGenerator)
        {
            _exceptions[type] = exceptionValueGenerator;
        }

        public virtual string CreateProperty(IComponentAddin addin, string propName, string selector)
        {
            if (addin.IsArrayedAddin)
                return CreateListTypeProperty(addin, propName, selector);
            return CreateSingleTypeProperty(addin, ref propName, selector);
        }

        private string CreateSingleTypeProperty(IComponentAddin addin, ref string propName, string selector)
        {
            string modifier = GetModifier(addin);
            propName = addin.Type == Consts.WEB_ELEMENT_CLASS_NAME ? propName + "Element" : propName;//looks more logical to me
            if (IsExceptionType(addin.Type))
                return $"{modifier} {addin.Type} {propName} => {HandleExcpetions(addin.Type, selector)};";

            return $"{modifier} {addin.Type} {propName} => new {addin.Type}({DriverPropertyName},{FindElementString(selector, false)});";
        }

        public virtual string CreateListTypeProperty(IComponentAddin addin, string propName, string selector)
        {
            string modifier = GetModifier(addin);
            return $"{modifier} ReadOnlyList<{addin.Type}> {propName} => {FindElementString(selector, true)};";
        }

        protected abstract string FindElementString(string selector, bool asList);

        private bool IsExceptionType(string type)
        {
            return _exceptions.ContainsKey(type);
        }
        private string HandleExcpetions(string type, string selector)
        {
            return _exceptions[type](selector);
        }

        private string GetModifier(IComponentAddin addin)
        {
            return addin.IsPropertyModifierPublic ? "public" : "protected";
        }
    }
}