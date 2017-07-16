using Core;

namespace BaseComponentsAddins
{
    public class LabelAddin : IComponentAddin
    {
        public string AddinKey => "label";

        public string Type => "string";

        public string[] RequiredUsings => new string[] { };

        public bool IsPropertyModifierPublic => true;

        public bool IsArrayedAddin => false;

        public bool CtorContainsDriver => false;

        public string[] GenerateHelpers(string className, string selector, IPropertyGenerator generator) => new string[] { };
    }
}
