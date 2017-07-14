﻿using System.Collections.Generic;
using System.Linq;
using SeleniumAutomationGenerator.Models;
using SeleniumAutomationGenerator.Utils;

namespace SeleniumAutomationGenerator.Generator
{
    public abstract class BasicClassGenerator : IClassGenerator
    {
        protected IAddinsContainer _container;
        protected IPropertyGenerator _propertiesGenerator;
        protected List<string> baseUsings;
        protected string _namespaceName;

        protected BasicClassGenerator(IAddinsContainer container, IPropertyGenerator propertyGenerator, string namespaceName)
        {
            baseUsings = new List<string>
            {
                "System", "OpenQA.Selenium"
            };

            _container = container;
            _namespaceName = namespaceName;
            _propertiesGenerator = propertyGenerator;
        }

        public virtual ComponentGeneratorOutput GenerateComponentClass(string className, ElementSelectorData[] elements)
        {
            BasicClassBuilder builder = new BasicClassBuilder();

            string body = builder.AddUsings(GetUsings(elements))
                .AddCtor(CreateCtor(className))
                .SetClassName(className)
                .SetNamesapce(_namespaceName)
                .AddUsings(GetUsings(elements))
                .AddProperties(GetProperties(elements))
                .AddMethods(GetHelpers(className, elements))
                .AddFields(GetFields())
                .Build();

            return new ComponentGeneratorOutput() { Body = body, CsFileName = NamespaceFileConverter.ConvertNamespaceToFilePath(_namespaceName, className) };
        }

        protected abstract string CreateCtor(string className);

        private string[] GetHelpers(string className, ElementSelectorData[] elements)
        {
            IEnumerable<string> helpers = new List<string>();
            foreach (var element in elements)
            {
                string[] innerHelpers = _container.GetAddin(element.Type).GenerateHelpers(className, element.Name);
                helpers = helpers.Concat(innerHelpers);
            }
            return helpers.ToArray();
        }

        protected virtual string[] GetProperties(ElementSelectorData[] elements)
        {
            return elements.Select(elm => _propertiesGenerator.CreateProperty(
                                            _container.GetAddin(elm.Type), elm.Name, elm.FullSelector))
                           .ToArray();
        }

        private string[] GetUsings(ElementSelectorData[] elements)
        {
            IEnumerable<string> usings = baseUsings;
            foreach (var element in elements)
            {
                usings = usings.Union(_container.GetAddin(element.Type).RequiredUsings);
            }
            return usings.ToArray();
        }

        protected virtual string[] GetFields()
        {
            return new string[] { };
        }
    }
}