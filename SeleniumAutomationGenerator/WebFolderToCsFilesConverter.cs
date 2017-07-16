using System.Collections.Generic;
using System.Linq;
using Core.Models;

namespace SeleniumAutomationGenerator
{
    public class WebFolderToCsFilesConverter
    {
        private readonly IComponentsFactory _factory;
        private readonly IHtmlsFinder _finder;

        public WebFolderToCsFilesConverter(IComponentsFactory factory , IHtmlsFinder finder)
        {
            _finder = finder;
            _factory = factory;
        }

        public List<ComponentGeneratorOutput> GenerateClasses(string baseDirectory)
        {
            List<ComponentGeneratorOutput> outputs = new List<ComponentGeneratorOutput>();
            foreach (var fileText in _finder.GetFilesTexts(baseDirectory))
            {
                outputs.AddRange(_factory.CreateCsOutput(fileText));
            }
            return outputs.Distinct(new ComponentOutputComparer()).ToList();
        }        
    }
}
