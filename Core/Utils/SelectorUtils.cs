namespace Core.Utils
{
    public static class SelectorUtils
    {
        public static string GetClassOrPropNameFromSelector(string selector)
        {
            string[] parts = selector.Split('-');
            return parts[2];
        }
        public static bool TryGetClassOrPropNameFromSelector(string selector,out string result)
        {
            string[] parts = selector.Split('-');
            if (parts.Length < 3)
            {
                result = null;
                return false;
            }
            result =  parts[2];
            return true;
        }
        public static string GetKeyWordFromSelector(string selector)
        {
            string[] parts = selector.Split('-');
            return parts[1];
        }
    }
}
