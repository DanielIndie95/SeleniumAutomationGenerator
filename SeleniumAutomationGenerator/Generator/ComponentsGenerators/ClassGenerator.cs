using System.Collections.Generic;
using System.Linq;
using Core;
using Core.Models;
using Core.Utils;
using SeleniumAutomationGenerator.Utils;
using System.Text;

namespace SeleniumAutomationGenerator.Generator.ComponentsGenerators
{
    public abstract class ClassGenerator : IComponentFileCreator
    {
        private readonly IClassBuilder _classBuilder;
        protected StringBuilder CtorBulk { get; }
        protected IAddinContainer Container;
        protected List<string> BaseUsings;
        protected string NamespaceName;
        protected List<string> ExtraProperties;
        protected List<string> ExtraMethods;

        public IPropertyGenerator PropertyGenerator { get; }
        protected virtual bool InheritFromBaseClass => true;

        protected ClassGenerator(IClassBuilder classBuilder, IPropertyGenerator propertyGenerator,IAddinContainer container, string namespaceName)
        {
            _classBuilder = classBuilder;
            BaseUsings = new List<string>
            {
                "System",
                "OpenQA.Selenium",
                "System.Linq"
            };
            ExtraProperties = new List<string>();
            ExtraMethods = new List<string>();
            Container = container;
            NamespaceName = namespaceName;
            PropertyGenerator = propertyGenerator;
            CtorBulk = new StringBuilder();
        }

        public virtual IComponentAddin MakeAddin(string selector)
        {
            string name = SelectorUtils.GetClassOrPropNameFromSelector(selector);
            return new FileCreatorAddin
            {
                AddinKey = name,
                RequiredUsings = new[] { NamespaceName }
            };
        }

        public virtual ComponentGeneratorOutput GenerateComponentClass(string selector, ElementSelectorData[] elements)
        {
            string className = SelectorUtils.GetClassOrPropNameFromSelector(selector);
            if (InheritFromBaseClass)
                _classBuilder.AddInheritance(Consts.DRIVER_CONTAINER_CLASS_NAME);
            string body = _classBuilder
                .AddUsings(GetUsings(elements))
                .AddCtor(CreateCtor(className))
                .SetClassName(className)
                .SetNamesapce(NamespaceName)
                .AddProperties(GetProperties(elements))
                .AddMethods(GetHelpers(className, elements))
                .AddFields(GetFields())
                .Build();

            return new ComponentGeneratorOutput
            {
                Body = body,
                CsFilePath = NamespaceFileConverter.ConvertNamespaceToFilePath(NamespaceName, className)
            };
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
            CtorBulk.AppendLine(bulk);
        }

        public void AddUsing(string usingName)
        {
            BaseUsings.Add(usingName);
        }

        protected abstract string CreateCtor(string className);

        protected virtual string[] GetFields()
        {
            return new string[] { };
        }

        protected virtual string[] GetProperties(ElementSelectorData[] elements)
        {
            return elements
                .SelectMany(GetProperties)
                .Concat(ExtraProperties)
                .Distinct()
                .ToArray();
        }

        private string[] GetHelpers(string className, IEnumerable<ElementSelectorData> elements)
        {
            IEnumerable<string> helpers = new List<string>();
            foreach (ElementSelectorData element in elements
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
            return Container.GetAddin(data.Type) != null;
        }

        private IEnumerable<string> GetHelpers(string className, ElementSelectorData element)
        {
            return Container.GetAddin(element.Type).GenerateHelpers(className, element.FullSelector, PropertyGenerator);
        }

        private IEnumerable<string> GetProperties(ElementSelectorData element)
        {
            if (element.AutomationAttributes.Length > 0)
            {
                KeyValuePair<Property, Property> propertyWithPrivateWebElement =
                    PropertyGenerator.CreatePropertyWithPrivateWebElement(
                        Container.GetAddin(element.Type) ?? DefaultAddin.Create(element.Type), element.Name,
                        element.FullSelector);
                return new string[]
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