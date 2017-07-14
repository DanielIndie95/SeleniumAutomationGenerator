using SeleniumAutomationGenerator.Models;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SeleniumAutomationGenerator.Generator
{
    public class BasicPageGenerator : IPageGenerator
    {
        IComponentsContainer _container;
        IPropertyGenerator _propertiesGenerator;

        string _namespaceName;

        public BasicPageGenerator(IComponentsContainer container, IPropertyGenerator propertyGenerator, string namespaceName)
        {
            _container = container;
            _namespaceName = namespaceName;
            _propertiesGenerator = propertyGenerator;
        }

        public string GeneratePageClass(string className, ElementSelectorData[] elements)
        {
            BasicClassBuilder builder = new BasicClassBuilder();

            return builder.AddUsings(GetUsings(elements))
                .AddCtor(CreateCtor(className))
                .SetClassName(className)
                .SetNamesapce(_namespaceName)
                .AddUsings("System", "OpenQA.Selenium")
                .AddProperties(GetProperties(elements))
                .AddMethods(GetHelpers(elements))
                .Build();
        }

        private string CreateCtor(string className)
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine($"internal {className}({Consts.WEB_DRIVER_CLASS_NAME} driver)");
            builder.AppendLine("{");
            builder.AppendLine("}");
            return builder.ToString();
        }
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

        private string[] GetProperties(ElementSelectorData[] elements)
        {
            return elements.Select(elm => _propertiesGenerator.CreateNode(
                                            _container.GetAddin(elm.Type), elm.Name, elm.FullSelector))
                           .ToArray();
        }

        private string[] GetUsings(ElementSelectorData[] elements)
        {
            IEnumerable<string> usings = new List<string>();
            foreach (var element in elements)
            {
                usings = usings.Union(_container.GetAddin(element.Type).RequiredUsings);
            }
            return usings.ToArray();
        }
    }
}
