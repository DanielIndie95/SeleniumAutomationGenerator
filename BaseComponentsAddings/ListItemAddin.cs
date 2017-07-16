using Core;

namespace BaseComponentsAddins
{
    public class ListItemAddin : IComponentAddin
    {
        public string AddinKey => "list";

        public string Type { get; set; }

        public string[] RequiredUsings => new string[] { };

        public bool IsPropertyModifierPublic => true;

        public bool IsArrayedAddin => true;

        public bool CtorContainsDriver { get; set; }

        public string[] GenerateHelpers(string className, string propName, IPropertyGenerator generator)
        {
            return new string[] { };
        }
    }
}
