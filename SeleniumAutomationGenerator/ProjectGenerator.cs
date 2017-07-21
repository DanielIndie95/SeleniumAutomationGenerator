using System.Collections.Generic;
using System.IO;
using Core.Models;
using SeleniumAutomationGenerator.Utils;
using SeleniumAutomationGenerator.Builders;
using System;
using Core.Utils;
using SeleniumAutomationGenerator.BaseGeneratedClasses;

namespace SeleniumAutomationGenerator
{
    public class ProjectGenerator
    {
        public void GenerateProject(string webAppBaseDirectory, string projectName, string solutionName = null,
            string distDirectory = null)
        {
            BuiltInComponentsInserter.InsertBuiltInComponents();
            distDirectory = distDirectory ?? Environment.CurrentDirectory + "\\test";
            CreateSolution(projectName);
            WebFolderToCsFilesConverter converter = new WebFolderToCsFilesConverter(ComponentsFactory.Instance, new HtmlFinder());
            List<ComponentGeneratorOutput> results = converter.GenerateClasses(webAppBaseDirectory);
            AddBaseClassesToResults(results);
            CreateFiles(distDirectory, results);
            string packageConfigName = CreatePackagesConfig(distDirectory);
            ProjectBuilder builder = new ProjectBuilder();
            builder.BuildProject(projectName, results, distDirectory, packageConfigName);
        }

        private static void CreateFiles(string distDirectory, List<ComponentGeneratorOutput> results)
        {
            Directory.CreateDirectory(distDirectory);
            Directory.CreateDirectory(distDirectory + "\\" + NamespaceFileConverter.ConvertNamespaceToFilePath(Consts.PAGES_NAMESPACE));
            Directory.CreateDirectory(distDirectory + "\\" + NamespaceFileConverter.ConvertNamespaceToFilePath(Consts.COMPONENTS_NAMESPACE));
            Directory.CreateDirectory(distDirectory + "\\" + NamespaceFileConverter.ConvertNamespaceToFilePath(Consts.BASE_NAMESPACE));
            foreach (ComponentGeneratorOutput result in results)
            {
                File.WriteAllText(distDirectory + "\\" + result.CsFilePath, result.Body);
            }
        }

        private void AddBaseClassesToResults(List<ComponentGeneratorOutput> results)
        {
            results.Add(BasicGeneratedClasses.DriverContainer);
        }

        private void CreateSolution(string name)
        {
        }

        private string CreatePackagesConfig(string distDirectory)
        {
            string packagesConfigBody = @"<?xml version=""1.0"" encoding=""utf-8""?>
  <packages>
        
         <package id = ""Selenium.Support"" version = ""2.48.0"" targetFramework = ""net452"" />
            
              <package id = ""Selenium.WebDriver"" version = ""2.48.0"" targetFramework = ""net452"" />
                 </packages> ";
            string packageConfigFilename = "packages.config";
            string fullPath = $"{distDirectory}\\{packageConfigFilename}";
            File.WriteAllText(fullPath , packagesConfigBody);
            return packageConfigFilename;
        }
    }
}