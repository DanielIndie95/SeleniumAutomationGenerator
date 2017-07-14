using System.Collections.Generic;
using System.Linq;
using SeleniumAutomationGenerator.Models;

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

        public virtual string GenerateComponentClass(string className, ElementSelectorData[] elements)
        {
            BasicClassBuilder builder = new BasicClassBuilder();

            return builder.AddUsings(GetUsings(elements))
                .AddCtor(CreateCtor(className))
                .SetClassName(className)
                .SetNamesapce(_namespaceName)
                .AddUsings(GetUsings(elements))
                .AddProperties(GetProperties(elements))
                .AddMethods(GetHelpers(elements))
                .AddFields(GetFields())
                .Build();
        }

        protected abstract string CreateCtor(string className);
       
        private string[] GetHelpers(ElementSelectorData[] elements)
        {
            IEnumerable<string> helpers = new List<string>();
            foreach (var element in elements)
            {
                string[] innerHelpers = _container.GetAddin(element.Type).GenerateHelpers(element.Name);
                helpers.Concat(innerHelpers);
            }
            return helpers.ToArray();
        }

        protected virtual string[] GetProperties(ElementSelectorData[] elements)
        {
            return elements.Select(elm => _propertiesGenerator.CreateNode(
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
