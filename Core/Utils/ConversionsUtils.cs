using Core.Models;

namespace Core.Utils
{
    public static class ConversionsUtils
    {
        public static ElementSelectorData ConvertToElementSelectorData(AutoElementData data)
        {
            return new ElementSelectorData
            {
                FullSelector = data.Selector,
                Name = SelectorUtils.GetClassOrPropNameFromSelector(data.Selector),
                Type = SelectorUtils.GetKeyWordFromSelector(data.Selector),
                AutomationAttributes = data.AutoAttributes
            };
        }
    }
}
