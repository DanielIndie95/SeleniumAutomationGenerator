using System.Collections.Generic;
using System.IO;
using Core.Models;

namespace SeleniumAutomationGenerator
{
    public class ProjectGenerator
    {
        public void GenerateProject(string webAppBaseDirectory, string projectName, string solutionName = null,
            string distDirectory = null)
        {
            CreateSolution(projectName);
            WebFolderToCsFilesConverter converter = new WebFolderToCsFilesConverter(ComponentsFactory.Instance , new HtmlFinder());
            List<ComponentGeneratorOutput> results = converter.GenerateClasses(webAppBaseDirectory);
            foreach (var result in results)
            {
                   File.WriteAllText(result.CsFileName , result.Body); 
            }
        }

        private void CreateSolution(string name)
        {
        }
    }
}