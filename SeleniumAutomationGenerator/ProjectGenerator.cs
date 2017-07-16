using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeleniumAutomationGenerator
{
    public class ProjectGenerator
    {
        public ProjectGenerator()
        {

        }
        public void GenerateProject(string webAppBaseDirectory, string projectName, string distDirectory = null)
        {
            CreateSolution(projectName);
            GenerateProject(webAppBaseDirectory, projectName, projectName);
        }
        public void GenerateProject(string webAppBaseDirectory, string projectName, string solutionName, string distDirectory = null)
        {

        }

        private void CreateSolution(string name)
        {

        }
    }
}
