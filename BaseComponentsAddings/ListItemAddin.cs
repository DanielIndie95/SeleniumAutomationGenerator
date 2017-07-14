using System;
using SeleniumAutomationGenerator;
using System.Text;

namespace BaseComponentsAddings
{
    public class ListItemAddin : IComponentAddin
    {
        public string AddinKey => "listItem";

        public string Type => "string";

        public string[] RequiredUsings => new string[] { };

        public bool IsPropertyModifierPublic => true;

        public bool IsArrayedAddin => true;

        public bool CtorContainsDriver => false;

        public string[] GenerateHelpers(string className, string propName)
        {
            return new string[] { };
        }
    }
}
