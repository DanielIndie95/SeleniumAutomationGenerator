namespace SeleniumAutomationGenerator.Utils
{
    public static class SelectorUtils
    {
        public static string GetClassNameFromSelector(string selector)
        {
            string[] parts = selector.Split('-');
            return parts[2];
        }
        public static string GetKeyWordFromSelector(string selector)
        {
            string[] parts = selector.Split('-');
            return parts[1];
        }
    }
}
