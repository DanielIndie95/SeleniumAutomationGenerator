using System.Collections.Generic;
using System.IO;
using Core.Models;
using SeleniumAutomationGenerator.Utils;
using SeleniumAutomationGenerator.Builders;
using System;
using Core.Utils;
using SeleniumAutomationGenerator.BaseGeneratedClasses;
using Core;

namespace SeleniumAutomationGenerator
{
    public class ProjectGenerator
    {
        public void GenerateProject(string webAppBaseDirectory, string projectName, string solutionName = null,
            string distDirectory = null)
        {
            BuiltInComponentsInserter.InsertBuiltInComponents(new BasicClassBuilder());
            distDirectory = distDirectory ?? Environment.CurrentDirectory + "\\test";
            CreateSolution(projectName);
            ComponentsContainer container = ComponentsContainer.Instance;
            ComponentsFactory componentsFactory = new ComponentsFactory(container, container, container, container);
            WebFolderToCsFilesConverter converter = new WebFolderToCsFilesConverter(componentsFactory, new HtmlFinder());
            List<ComponentGeneratorOutput> results = converter.GenerateClasses(webAppBaseDirectory);
            AddBaseClassesToResults(results);
            CreateFiles(distDirectory, results);
            string packageConfigName = CreatePackagesConfig(distDirectory);
            ProjectBuilder builder = new ProjectBuilder();
            builder.BuildProject(projectName, results, distDirectory, packageConfigName);
        }

        private static void CreateFiles(string distDirectory, IEnumerable<ComponentGeneratorOutput> results)
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

        private static void AddBaseClassesToResults(ICollection<ComponentGeneratorOutput> results)
        {
            results.Add(BasicGeneratedClasses.DriverContainer);
        }

        // ReSharper disable once UnusedParameter.Local
        private void CreateSolution(string name)
        {
        }

        private static string CreatePackagesConfig(string distDirectory)
        {
            const string PACKAGES_CONFIG_BODY = @"<?xml version=""1.0"" encoding=""utf-8""?>
  <packages>
        
         <package id = ""Selenium.Support"" version = ""2.48.0"" targetFramework = ""net452"" />
            
              <package id = ""Selenium.WebDriver"" version = ""2.48.0"" targetFramework = ""net452"" />
                 </packages> ";
            const string PACKAGE_CONFIG_FILENAME = "packages.config";
            string fullPath = $"{distDirectory}\\{PACKAGE_CONFIG_FILENAME}";
            File.WriteAllText(fullPath, PACKAGES_CONFIG_BODY);
            return PACKAGE_CONFIG_FILENAME;
        }
    }
}