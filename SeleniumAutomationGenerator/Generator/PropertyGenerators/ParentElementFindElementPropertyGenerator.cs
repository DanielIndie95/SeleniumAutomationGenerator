namespace SeleniumAutomationGenerator.Generator.PropertyGenerators
{
    public class ParentElementFindElementPropertyGenerator : SearchContextFindElementPropertyGenerator
    {
        private readonly string _parentElementField;
        public ParentElementFindElementPropertyGenerator(string driverElementName, string parentElementName) : base(driverElementName)
        {
            _parentElementField = parentElementName;
        }

        protected override string FindElementString(string selector, bool asList)
        {
            string add = asList ? "s" : "";
            return $"{_parentElementField}.FindElement{add}(By.ClassName(\"{selector}\"))";
        }
    }
}
