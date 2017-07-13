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
        public BasicPageGenerator(IComponentsContainer container, string namespaceName, IPropertyGenerator propertyGenerator)
        {
            _container = container;
            _namespaceName = namespaceName;
            _propertiesGenerator = propertyGenerator;
        }

        public string GeneratePageClass(string className, ElementSelectorData[] elements)
        {
            StringBuilder builder = new StringBuilder();
            AppendUsings(elements, builder);
            StringBuilder classBuilder = CreateClassBuilder(className, elements);
            AppendNamespace(builder, CreateClassBuilder(className, elements));
            return builder.ToString();
        }

        private StringBuilder CreateClassBuilder(string className, ElementSelectorData[] elements)
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine($"public class {className}");
            builder.AppendLine("{");
            AppendProperties(builder, elements);
            AppendCtor(builder, className);
            AppendHelpers(builder, elements);
            builder.AppendLine("}");
            return builder;
        }

        private void AppendHelpers(StringBuilder builder, ElementSelectorData[] elements)
        {
            foreach (var element in elements)
            {
                string[] helpers = _container.GetAddin(element.Type).GenerateHelpers(element.Name);
                foreach (var helper in helpers)
                {
                    builder.AppendLine(helper);
                }
            }
        }

        private void AppendCtor(StringBuilder builder, string className)
        {
            builder.AppendLine($"internal {className}({Consts.WEB_DRIVER_CLASS_NAME} driver)");
            builder.AppendLine("{");
            builder.AppendLine("}");
        }

        private void AppendProperties(StringBuilder builder, ElementSelectorData[] elements)
        {
            foreach (var element in elements)
            {
                string property = _propertiesGenerator.CreateNode(_container.GetAddin(element.Type), element.Name, element.FullSelector);
                builder.AppendLine(property);
            }
        }

        private void AppendNamespace(StringBuilder builder, StringBuilder classBody)
        {
            builder.AppendLine($"namespace {_namespaceName}");
            builder.Append("{");
            builder.Append(classBody);
            builder.AppendLine("}");
        }

        private void AppendUsings(ElementSelectorData[] elements, StringBuilder builder)
        {
            string[] usings = GetUsings(elements);
            foreach (var usingNamespace in usings)
            {
                builder.AppendLine($"using {usingNamespace};");
            }
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
