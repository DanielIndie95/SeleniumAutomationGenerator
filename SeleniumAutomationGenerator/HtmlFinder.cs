﻿using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SeleniumAutomationGenerator
{
    public class HtmlFinder : IHtmlsFinder
    {
        public string[] GetFilesTexts(string baseDirectory)
        {
            return FindHtmlFiles(baseDirectory)
                .Select(file => File.ReadAllText(file)).ToArray();
        }

        private IEnumerable<string> FindHtmlFiles(string baseDirectory)
        {
            return Directory.EnumerateFiles(
                baseDirectory, "*.*", SearchOption.AllDirectories);            
        }
    }
}
