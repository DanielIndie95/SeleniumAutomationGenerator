using Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Models;
using Core.Utils;
using SeleniumAutomationGenerator.Generator.ComponentsGenerators;

namespace SeleniumAutomationGenerator.Generator.CustomClassAttributes
{
    public class WaitUntilDisplayedElementAttribute : IElementAttribute
    {
        public string Name => "auto-wait-displayed";
        private string GetWaiterBulk(string selector)
        {
            string waiterFieldName = "wait" + SelectorUtils.GetClassOrPropNameFromSelector(selector);
            string waiter = $"var {waiterFieldName} = new WebDriverWait({Consts.DRIVER_FIELD_NAME});";
            string waiting = $"{waiterFieldName}.Until(ExpectedConditions.ElementIsVisible({selector}))";
            return new StringBuilder()
                .AppendLine(waiter)
                .AppendLine(waiting)
                .ToString();
        }
        public void AppendToClass(IComponentFileCreator parentClass, AutoElementData appenderElement)
        {
            string ctorBulk =  GetWaiterBulk(appenderElement.Selector);
            parentClass.InsertToCtor(ctorBulk);
        }
    }
}
