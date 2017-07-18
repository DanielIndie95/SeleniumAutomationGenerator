using System.Collections.Generic;
using System.Linq;
using Core.Utils;
using HtmlAgilityPack;
using SeleniumAutomationGenerator.Models;

namespace SeleniumAutomationGenerator.Utils
{
    public static class AutoElementFinder
    {
        public static IEnumerable<AutoElementData> GetChildren(string body)
        {
            var doc = new HtmlDocument();
            HtmlNode.ElementsFlags.Remove("form");
            doc.LoadHtml(body);

            HtmlNode[] nodes = doc.DocumentNode.ChildNodes
                .Where(node => node.Attributes["class"]?.Value.Contains(Consts.AUTOMATION_ELEMENT_PREFIX) ?? false)
                .ToArray();
            if (nodes.Length == 0)
                yield break;
            
            foreach (HtmlNode node in nodes)
            {
                AutoElementData data = new AutoElementData
                {
                    Selector = FindAutoClassFromFullClass(node.Attributes["class"].Value),
                    InnerChildrens = GetChildren(node.InnerHtml).ToList(),
                    AutoAttributes = node.Attributes.Where(att => att.Name.StartsWith("auto-"))
                        .Select(att => att.Name)
                        .ToArray()
                };
                yield return data;
            }
        }

        private static string FindAutoClassFromFullClass(string fullClass)
        {
            return fullClass.Split(' ').First(cls => cls.StartsWith(Consts.AUTOMATION_ELEMENT_PREFIX));
        }
    }
}
