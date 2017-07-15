using System.Collections.Generic;
using System.Linq;
using SeleniumAutomationGenerator.Models;
using SeleniumAutomationGenerator.Utils;
using SeleniumAutomationGenerator.Generator.ComponentsGenerators;

namespace SeleniumAutomationGenerator.Generator
{
    public abstract class BasicClassGenerator : IComponentFileCreator
    {
        protected ComponentsContainer _container;
        protected IPropertyGenerator _propertyGenerator;
        protected List<string> baseUsings;
        protected string _namespaceName;
        protected List<string> ExceptionsTypes;
        protected List<string> ExtraProperties;
        protected List<string> ExtraMethods;

        public IPropertyGenerator PropertyGenerator => _propertyGenerator;

        protected BasicClassGenerator(ComponentsContainer container, IPropertyGenerator propertyGenerator, string namespaceName)
        {
            baseUsings = new List<string>
            {
                "System", "OpenQA.Selenium", "System.Linq"
            };
            ExceptionsTypes = new List<string>();
            ExtraProperties = new List<string>();
            ExtraMethods = new List<string>();
            _container = container;
            _namespaceName = namespaceName;
            _propertyGenerator = propertyGenerator;
        }

        public virtual IComponentAddin MakeAddin(string selector)
        {
            string name = SelectorUtils.GetClassOrPropNameFromSelector(selector);
            return new FileCreatorAddin()
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
                .SetNamesapce(_namespaceName)
                .AddUsings(GetUsings(elements))
                .AddProperties(GetProperties(elements))
                .AddMethods(GetHelpers(className, elements))
                .AddFields(GetFields())
                .Build();

            return new ComponentGeneratorOutput() { Body = body, CsFileName = NamespaceFileConverter.ConvertNamespaceToFilePath(_namespaceName, className) };
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

        protected abstract string CreateCtor(string className);

        private string[] GetHelpers(string className, ElementSelectorData[] elements)
        {
            IEnumerable<string> helpers = new List<string>();
            foreach (var element in elements.Where(elm => !ExceptionsTypes.Contains(elm.Type))
                .Where(ExistingTypes))
            {
                string[] innerHelpers = _container.GetAddin(element.Type).GenerateHelpers(className, element.FullSelector, PropertyGenerator);
                helpers = helpers.Concat(innerHelpers);
            }
            return helpers.ToArray();
        }

        protected virtual string[] GetProperties(ElementSelectorData[] elements)
        {
            return elements
                            .Where(elm => !ExceptionsTypes.Contains(elm.Type))
                            //.Where(ExistingTypes)
                            .Select(elm => _propertyGenerator.CreateProperty(
                                            _container.GetAddin(elm.Type) ?? DefaultAddin.Create(elm.Type), elm.Name, elm.FullSelector))
                           .Concat(ExtraProperties)
                           .Distinct()
                           .ToArray();
        }

        private string[] GetUsings(ElementSelectorData[] elements)
        {
            IEnumerable<string> usings = baseUsings;
            foreach (var element in elements.Where(ExistingTypes))
            {
                usings = usings.Concat(_container.GetAddin(element.Type).RequiredUsings);
            }
            return usings.ToArray();
        }

        protected virtual string[] GetFields()
        {
            return new string[] { };
        }
        private bool ExistingTypes(ElementSelectorData data)
        {
            bool result = _container.GetAddin(data.Type) != null;
            return result;
        }
    }
}
