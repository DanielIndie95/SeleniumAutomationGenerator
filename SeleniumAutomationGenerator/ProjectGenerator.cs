using System.Collections.Generic;
using System.IO;
using Core.Models;
using SeleniumAutomationGenerator.Utils;

namespace SeleniumAutomationGenerator
{
    public class ProjectGenerator
    {
        public void GenerateProject(string webAppBaseDirectory, string projectName, string solutionName = null,
            string distDirectory = null)
        {
            BuiltInComponentsInserter.InsertBuiltInComponents();
            
            CreateSolution(projectName);
            WebFolderToCsFilesConverter converter = new WebFolderToCsFilesConverter(ComponentsFactory.Instance , new HtmlFinder());
            List<ComponentGeneratorOutput> results = converter.GenerateClasses(webAppBaseDirectory);
            foreach (ComponentGeneratorOutput result in results)
            {
                   File.WriteAllText(result.CsFileName , result.Body); 
            }
        }

        

        private void CreateSolution(string name)
        {
        }
    }
}