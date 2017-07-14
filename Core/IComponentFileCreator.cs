﻿using SeleniumAutomationGenerator.Models;

namespace SeleniumAutomationGenerator.Generator
{
    public interface IComponentFileCreator
    {
        IPropertyGenerator PropertyGenerator { get; }
        ComponentGeneratorOutput GenerateComponentClass(string selector, ElementSelectorData[] elements);
        void AddExceptionPropertyType(string type);
        void AddProperty(string property);
        void AddMethod(string method);
        IComponentAddin MakeAddin(string selector);
    }
}
