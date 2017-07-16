namespace SeleniumAutomationGenerator
{
    public class ProjectGenerator
    {
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
