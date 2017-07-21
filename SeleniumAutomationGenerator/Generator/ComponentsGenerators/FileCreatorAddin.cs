using System;
using Core;

namespace SeleniumAutomationGenerator.Generator.ComponentsGenerators
{
    public class FileCreatorAddin : IComponentAddin
    {
        private readonly Func<string, string, IPropertyGenerator, string[]> _helpers;

        public string AddinKey { get; set; }

        public string Type { get; set; }

        public string[] RequiredUsings { get; set; } = { };

        public bool IsPropertyModifierPublic { get; set; } = true;

        public bool IsArrayedAddin { get; set; } = false;

        public bool CtorContainsDriver { get; set; } = true;

        public FileCreatorAddin(Func<string, string, IPropertyGenerator, string[]> helpers)
        {
            _helpers = helpers;
        }
        public FileCreatorAddin()
        {
            _helpers = (a, b, c) => new string[] { };
        }

        public string[] GenerateHelpers(string className, string selector, IPropertyGenerator generator)
        {
            return _helpers(className, selector, generator);
        }
    }
}
