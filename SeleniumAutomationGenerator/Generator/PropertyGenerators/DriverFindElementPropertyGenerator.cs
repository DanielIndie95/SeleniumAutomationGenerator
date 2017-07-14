using SeleniumAutomationGenerator.Generator;

namespace SeleniumAutomationGenerator
{
    public class DriverFindElementPropertyGenerator : SearchContextFindElementPropertyGenerator
    {
        public DriverFindElementPropertyGenerator(string driverElementName) : base(driverElementName)
        {
        }

        protected override string FindElementString(string selector, bool asList)
        {
            string add = asList ? "s" : "";
            return $"{DriverPropertyName}.FindElement{add}(By.ClassName(\"{selector}\"))";
        }
    }
}
