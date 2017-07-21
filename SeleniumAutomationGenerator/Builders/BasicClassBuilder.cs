using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core;
using Core.Models;
using Core.Utils;

namespace SeleniumAutomationGenerator.Builders
{
    public class BasicClassBuilder : IClassBuilder
    {
        private string _ctor;
        private List<string> _usings;
        private readonly Dictionary<Modifiers, List<string>> _fields;
        private readonly Dictionary<Modifiers, List<string>> _props;
        private readonly Dictionary<Modifiers, List<string>> _methods;
        private string _namespaceName;
        private string _className;
        private List<string> _inheritance;

        public BasicClassBuilder()
        {
            _usings = new List<string>()
            { Consts.BASE_NAMESPACE};
            _inheritance = new List<string>();
            _fields = new Dictionary<Modifiers, List<string>>();
            _props = new Dictionary<Modifiers, List<string>>();
            _methods = new Dictionary<Modifiers, List<string>>();
        }

        public static string CreateField(string type, string name, Modifiers modifier = Modifiers.Private)
        {
            return $"{modifier.ToString().ToLower()} {type} {name};";
        }

        public IClassBuilder AddCtor(string ctor)
        {
            _ctor = ctor;
            return this;
        }

        public IClassBuilder AddFields(params string[] fields)
        {
            foreach (string field in fields)
            {
                AddModifierField(_fields, field);
            }

            return this;
        }

        public IClassBuilder AddMethods(params string[] methods)
        {
            foreach (string method in methods)
            {
                AddModifierField(_methods, method);
            }

            return this;
        }

        public IClassBuilder AddProperties(params string[] properties)
        {
            foreach (string property in properties)
            {
                AddModifierField(_props, property);
            }

            return this;
        }

        public IClassBuilder AddUsings(params string[] usings)
        {
            _usings = _usings.Union(usings).ToList();

            return this;
        }

        public IClassBuilder SetNamesapce(string namespaceName)
        {
            _namespaceName = namespaceName;
            return this;
        }

        public IClassBuilder SetClassName(string className)
        {
            _className = className;
            return this;
        }

        public IClassBuilder AddInheritance(params string[] types)
        {
            _inheritance.AddRange(types.OrderBy(type => !type.StartsWith("I"))); //I - interfaces
            return this;
        }

        public string Build()
        {
            StringBuilder builder = new StringBuilder();
            AppendUsings(builder, _usings);
            StringBuilder classBuilder = CreateClassBuilder();
            AppendNamespace(builder, classBuilder);
            return builder.ToString();
        }
        public string Build(bool inheritBaseClass)
        {
            StringBuilder builder = new StringBuilder();
            AppendUsings(builder, _usings);
            StringBuilder classBuilder = CreateClassBuilder();
            AppendNamespace(builder, classBuilder);
            return builder.ToString();
        }

        private StringBuilder CreateClassBuilder()
        {
            StringBuilder builder = new StringBuilder();
            if (_className == null)
                throw new InvalidOperationException("class name not found");

            string inheritance = _inheritance.Any() ? ":" + string.Join(",", _inheritance) : "";
            builder.AppendLine($"public class {_className} {inheritance}");
            builder.AppendLine("{");
            AppendFields(builder);
            AppendProperties(builder);
            builder.Append(_ctor);
            AppendMethods(builder);
            builder.AppendLine("}");
            return builder;
        }

        private void AppendNamespace(StringBuilder builder, StringBuilder classBody)
        {
            if (_namespaceName != null)
                builder.AppendLine($"namespace {_namespaceName}");
            builder.AppendLine("{");
            builder.Append(classBody);
            builder.AppendLine("}");
        }

        private void AppendFields(StringBuilder builder)
        {
            AppendModifier(builder, _fields, Modifiers.Public);
            AppendModifier(builder, _fields, Modifiers.Protected);
            AppendModifier(builder, _fields, Modifiers.Private);
        }

        private void AppendMethods(StringBuilder builder)
        {
            AppendModifier(builder, _methods, Modifiers.Public);
            AppendModifier(builder, _methods, Modifiers.Protected);
            AppendModifier(builder, _methods, Modifiers.Private);
        }

        private void AppendProperties(StringBuilder builder)
        {
            AppendModifier(builder, _props, Modifiers.Private);
            AppendModifier(builder, _props, Modifiers.Protected);
            AppendModifier(builder, _props, Modifiers.Public);
        }

        private void AppendUsings(StringBuilder builder, IEnumerable<string> usings)
        {
            foreach (string usingNamespace in usings)
            {
                builder.AppendLine($"using {usingNamespace};");
            }
        }

        private void AppendModifier(StringBuilder builder, Dictionary<Modifiers, List<string>> fields, Modifiers modifier)
        {
            if (fields.ContainsKey(modifier))
                AppendList(builder, fields[modifier]);
        }

        private void AddModifierField(Dictionary<Modifiers, List<string>> container, string newField)
        {
            Modifiers modifier = FindModifier(newField);
            if (!container.TryGetValue(modifier, out List<string> fields))
                container[modifier] = fields = new List<string>();

            fields.Add(newField);
        }

        private Modifiers FindModifier(string field)
        {
            string firstWord = field.Split(' ').First();

            return Enum.TryParse(TextUtils.UppercaseFirst(firstWord), out Modifiers modifier)
                ? modifier : Modifiers.Private;
        }

        private void AppendList<T>(StringBuilder builder, IEnumerable<T> list)
        {
            foreach (T item in list)
            {
                builder.AppendLine(item.ToString());
            }
        }

    }
}
