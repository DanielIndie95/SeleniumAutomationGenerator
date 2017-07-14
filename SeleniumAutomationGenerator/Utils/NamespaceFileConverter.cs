﻿using System.Linq;

namespace SeleniumAutomationGenerator.Utils
{
    public class NamespaceFileConverter
    {
        public static string ConvertNamespaceToFilePath(string nameSpace, string className = null)
        {
            string result = nameSpace.Replace(".", "\\") + (className != null ? $"\\{className}.cs" : "");
            return result;
        }
        public static string ConvertFilePathNamespace(string filePath)
        {
            string[] splitted = filePath.Split('\\');
            if (splitted.Last().Contains(".cs"))
                return string.Join(".", splitted.Take(splitted.Length - 1));
            return filePath.Replace("\\", ".");
        }
    }
}
