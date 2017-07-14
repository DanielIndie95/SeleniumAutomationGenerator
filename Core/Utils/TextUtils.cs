using System;

namespace Core.Utils
{
    public class TextUtils
    {
        public static string UppercaseFirst(string s)
        {
            // Check for empty string.
            if (string.IsNullOrEmpty(s))
            {
                return string.Empty;
            }
            // Return char and concat substring.
            return char.ToUpper(s[0]) + s.Substring(1);
        }

        public static string ToCamelCase(string s)
        {
            return Char.ToLowerInvariant(s[0]) + s.Substring(1);
        }
    }
}
