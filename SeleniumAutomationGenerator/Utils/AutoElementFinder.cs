using HtmlAgilityPack;
using SeleniumAutomationGenerator.Models;
using System.Collections.Generic;
using System.Linq;

namespace SeleniumAutomationGenerator.Utils
{
    public static class AutoElementFinder
    {
        public static IEnumerable<AutoElementData> GetChildren(string body)
        {
            var doc = new HtmlDocument();
            HtmlNode.ElementsFlags.Remove("form");
            doc.LoadHtml(body);

            HtmlNodeCollection collection = doc.DocumentNode.SelectNodes($"//*[contains(@class, '{Consts.AUTOMATION_ELEMENT_PREFIX}')]");
            if (collection == null)
                yield break;
            foreach (var node in collection)
            {
                AutoElementData data = new AutoElementData();
                data.Selector = FindAutoClassFromFullClass(node.Attributes["class"].Value);
                data.InnerChildrens = GetChildren(node.InnerHtml).ToList();
                yield return data;
            }
        }

        private static string FindAutoClassFromFullClass(string fullClass)
        {
            return fullClass.Split(' ').First(cls => cls.StartsWith(Consts.AUTOMATION_ELEMENT_PREFIX));
        }
    }
}
