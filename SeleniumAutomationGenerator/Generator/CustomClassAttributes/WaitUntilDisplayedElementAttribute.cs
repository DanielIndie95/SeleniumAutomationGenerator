using Core;
using System.Text;
using Core.Models;
using Core.Utils;

namespace SeleniumAutomationGenerator.Generator.CustomClassAttributes
{
    public class WaitUntilDisplayedElementAttribute : IElementAttribute
    {
        public string Identifier => "auto-wait-displayed";
        private string GetWaiterBulk(string selector)
        {
            string waiterFieldName = "wait" + SelectorUtils.GetClassOrPropNameFromSelector(selector);
            string waiter = $"var {waiterFieldName} = new WebDriverWait({Consts.DRIVER_FIELD_NAME},TimeSpan.FromSeconds(5));";
            string waiting = $"{waiterFieldName}.Until(ExpectedConditions.ElementIsVisible(By.ClassName(\"{selector}'\")));";
            return new StringBuilder()
                .AppendLine(waiter)
                .AppendLine(waiting)
                .ToString();
        }
        public void AppendToClass(IComponentFileCreator parentClass, AutoElementData appenderElement)
        {
            string ctorBulk =  GetWaiterBulk(appenderElement.Selector);
            parentClass.InsertToCtor(ctorBulk);
            parentClass.AddUsing("OpenQA.Selenium.Support.UI");
        }
    }
}
