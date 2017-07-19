using System.Collections.Generic;

namespace SeleniumAutomationGenerator.Models
{
    public class AutoElementData
    {
        public string Selector { get; set; }

        public IEnumerable<AutoElementData> InnerChildrens { get; set; }

        public string[] AutoAttributes { get; set; }
    }
}
