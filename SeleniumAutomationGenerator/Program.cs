using System;

namespace SeleniumAutomationGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            string projectName = args[0];
            string webAppBaseDirectory = args.Length > 1 ? args[1] : Environment.CurrentDirectory;
            ProjectGenerator generator = new ProjectGenerator();
            generator.GenerateProject(webAppBaseDirectory, projectName);
        }
    }
}