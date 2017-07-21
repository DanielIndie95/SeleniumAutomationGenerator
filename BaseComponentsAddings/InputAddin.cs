using Core.Utils;
using System.Text;
using Core;

namespace BaseComponentsAddins
{
    public class InputAddin : IComponentAddin
    {
        public string AddinKey => "input";

        public string Type => Consts.WEB_ELEMENT_CLASS_NAME;

        public string[] RequiredUsings => new string[] { };

        public bool IsPropertyModifierPublic => true;

        public bool IsArrayedAddin => false;

        public bool CtorContainsDriver => false;

        public string[] GenerateHelpers(string className, string selector, IPropertyGenerator generator)
        {
            string propName = SelectorUtils.GetClassOrPropNameFromSelector(selector);
            string propertyNameFromGenerator = generator.GetPropertyName(Type, propName);
            string methodPropName = TextUtils.UppercaseFirst(propName);
            StringBuilder builder = new StringBuilder();
            string helper = builder
                .AppendLine($"public {className} With{methodPropName}(string {propName})")
                .AppendLine("{")
                .AppendLine($"{propertyNameFromGenerator}.SendKeys({propName});")
                .AppendLine("return this;")
                .AppendLine("}")
                .ToString();
            return new [] { helper };
        }
    }
}
