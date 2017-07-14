using System;

namespace SeleniumAutomationGenerator.Generator.ComponentsGenerators
{
    public class FileCreatorAddin : IComponentAddin
    {
        private Func<string, string, string[]> _helpers;

        public string AddinKey { get; set; }

        public string Type { get; set; }

        public string[] RequiredUsings { get; set; } = new string[] { };

        public bool IsPropertyModifierPublic { get; set; } = true;

        public bool IsArrayedAddin { get; set; } = false;

        public bool CtorContainsDriver { get; set; } = true;

        public FileCreatorAddin(Func<string, string, string[]> helpers)
        {
            _helpers = helpers;
        }
        public FileCreatorAddin()
        {
            _helpers = (a, b) => new string[] { };
        }

        public string[] GenerateHelpers(string className, string selector, IPropertyGenerator generator)
        {
            return _helpers(className, selector);
        }

    }
}
