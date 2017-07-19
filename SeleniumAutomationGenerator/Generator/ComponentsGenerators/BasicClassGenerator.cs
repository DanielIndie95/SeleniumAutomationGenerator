using System;
using System.Collections.Generic;
using System.Linq;
using Core;
using Core.Models;
using Core.Utils;
using SeleniumAutomationGenerator.Generator.Builders;
using SeleniumAutomationGenerator.Utils;

namespace SeleniumAutomationGenerator.Generator.ComponentsGenerators
{
    public abstract class BasicClassGenerator : IComponentFileCreator
    {
        protected ComponentsContainer Container;
        protected List<string> BaseUsings;
        protected string NamespaceName;
        protected List<string> ExceptionsTypes;
        protected List<string> ExtraProperties;
        protected List<string> ExtraMethods;

        public IPropertyGenerator PropertyGenerator { get; }

        protected BasicClassGenerator(IPropertyGenerator propertyGenerator, string namespaceName)
        {
            BaseUsings = new List<string>
            {
                "System",
                "OpenQA.Selenium",
                "System.Linq"
            };
            ExceptionsTypes = new List<string>();
            ExtraProperties = new List<string>();
            ExtraMethods = new List<string>();
            Container = ComponentsContainer.Instance;
            NamespaceName = namespaceName;
            PropertyGenerator = propertyGenerator;
        }

        public virtual IComponentAddin MakeAddin(string selector)
        {
            string name = SelectorUtils.GetClassOrPropNameFromSelector(selector);
            return new FileCreatorAddin
            {
                AddinKey = name,
                Type = name
            };
        }

        public virtual ComponentGeneratorOutput GenerateComponentClass(string selector, ElementSelectorData[] elements)
        {
            BasicClassBuilder builder = new BasicClassBuilder();
            string className = SelectorUtils.GetClassOrPropNameFromSelector(selector);
            string body = builder.AddUsings(GetUsings(elements))
                .AddCtor(CreateCtor(className))
                .SetClassName(className)
                .SetNamesapce(NamespaceName)
                .AddUsings(GetUsings(elements))
                .AddProperties(GetProperties(elements))
                .AddMethods(GetHelpers(className, elements))
                .AddFields(GetFields())
                .Build();

            return new ComponentGeneratorOutput
            {
                Body = body,
                CsFileName = NamespaceFileConverter.ConvertNamespaceToFilePath(NamespaceName, className)
            };
        }

        public void AddExceptionPropertyType(string type)
        {
            ExceptionsTypes.Add(type);
        }

        public void AddProperty(string property)
        {
            ExtraProperties.Add(property);
        }

        public void AddMethod(string method)
        {
            ExtraMethods.Add(method);
        }

        public void InsertToCtor(string bulk)
        {
            throw new NotImplementedException();
        }

        protected abstract string CreateCtor(string className);

        protected virtual string[] GetFields()
        {
            return new string[] { };
        }

        protected virtual string[] GetProperties(ElementSelectorData[] elements)
        {
            return elements
                .Where(elm => !ExceptionsTypes.Contains(elm.Type))
                .SelectMany(GetProperties)
                .Concat(ExtraProperties)
                .Distinct()
                .ToArray();
        }

        private string[] GetHelpers(string className, IEnumerable<ElementSelectorData> elements)
        {
            IEnumerable<string> helpers = new List<string>();
            foreach (ElementSelectorData element in elements.Where(elm => !ExceptionsTypes.Contains(elm.Type))
                .Where(ExistingTypes))
            {
                IEnumerable<string> innerHelpers = GetHelpers(className, element);
                helpers = helpers.Concat(innerHelpers);
            }
            return helpers.ToArray();
        }        

        private string[] GetUsings(IEnumerable<ElementSelectorData> elements)
        {
            IEnumerable<string> usings = elements.Where(ExistingTypes)
                .Aggregate<ElementSelectorData, IEnumerable<string>>(BaseUsings,
                    (current, element) => current.Concat(Container.GetAddin(element.Type).RequiredUsings));
            return usings.ToArray();
        }        

        private bool ExistingTypes(ElementSelectorData data)
        {
            bool result = Container.GetAddin(data.Type) != null;
            return result;
        }

        private IEnumerable<string> GetHelpers(string className,ElementSelectorData element)
        {            
            return Container.GetAddin(element.Type).GenerateHelpers(className,element.FullSelector,PropertyGenerator);
        }

        private IEnumerable<string> GetProperties(ElementSelectorData element)
        {
            if (element.AutomationAttributes.Length > 0)
            {
                KeyValuePair<Property, Property> propertyWithPrivateWebElement =
                    PropertyGenerator.CreatePropertyWithPrivateWebElement(
                        Container.GetAddin(element.Type) ?? DefaultAddin.Create(element.Type), element.Name,
                        element.FullSelector);
                return new List<string>
                {
                    propertyWithPrivateWebElement.Key,
                    propertyWithPrivateWebElement.Value                    
                };                
                
            }
            return new string[]
            {
                PropertyGenerator.CreateProperty(
                    Container.GetAddin(element.Type) ?? DefaultAddin.Create(element.Type), element.Name,
                    element.FullSelector)
            };
        }        
    }
}