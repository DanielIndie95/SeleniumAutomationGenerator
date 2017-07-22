using BaseComponentsAddins;
using Core;
using Core.Models;
using Core.Utils;
using System.Linq;

namespace SeleniumAutomationGenerator.Generator.ClassAppenders
{
    public class ListClassAppender : IComponentClassAppender
    {
        private readonly IAddinContainer _addinContainer;
        public ListClassAppender(IAddinContainer addinContainer)
        {
            _addinContainer = addinContainer;
        }
        public string Identifier => "list";

        public void AppendToClass(IComponentFileCreator parentClass, AutoElementData appenderElement)
        {
            ElementSelectorData[] elements = appenderElement.InnerChildrens.Select(ConversionsUtils.ConvertToElementSelectorData)
                .ToArray();
            bool ctorContainsDriver;
            string type;
            string selector = appenderElement.Selector;
            if (elements.Length > 0)
            {
                var elementAddin = _addinContainer.GetAddin(elements[0].Type);
                type = elementAddin != null ? elementAddin.Type : elements[0].Name;
                selector = elements[0].FullSelector;
                ctorContainsDriver = _addinContainer.GetAddin(type)?.CtorContainsDriver ?? false;
            }
            else
            {
                type = "string";
                ctorContainsDriver = false;
            }
            string name = SelectorUtils.TryGetClassOrPropNameFromSelector(appenderElement.Selector, out name) ? name : type + "List";
            ListItemAddin addin = new ListItemAddin
            {
                Type = type,
                CtorContainsDriver = ctorContainsDriver
            };

            parentClass.AddProperty(parentClass.PropertyGenerator.CreateProperty(addin, name, selector));
        }
    }
}
