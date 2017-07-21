using System.IO;
using Core.Models;
using Microsoft.Build.Construction;
using System.Collections.Generic;
using System.Linq;

namespace SeleniumAutomationGenerator.Builders
{
    public class ProjectBuilder
    {
        public void BuildProject(string projectName, IEnumerable<ComponentGeneratorOutput> files , string csprojDir , string packageConfigFilename)
        {
            var root = ProjectRootElement.Create();
            AddProperties(projectName, root);

            // references           
            AddItems(root, "Reference", "System", "System.Core", "System.Drawing", "System.Xml.Linq", "System.Data.DataSetExtensions", "Microsoft.CSharp", "System.Data", "System.Net.Http", "System.Xml");
            // items to compile
            AddItems(root, "Compile", files.Select(file => file.CsFilePath).ToArray());

            AddItems(root, "None", packageConfigFilename);
            var target = root.AddTarget("Build");
            var task = target.AddTask("Csc");
            task.SetParameter("Sources", "@(Compile)");
            task.SetParameter("OutputAssembly", $"{projectName}.dll");
            AddImports(root);

            root.Save($"{csprojDir}\\{projectName}.csproj");
            var s = File.ReadAllText($"{csprojDir}\\{projectName}.csproj");
        }

        private static void AddProperties(string projectName, ProjectRootElement root)
        {
            var group = root.AddPropertyGroup();
            group.AddProperty("Configuration", "Debug");
            group.AddProperty("Platform", "AnyCPU");
            group.AddProperty("OutputType", "Library");
            group.AddProperty("AppDesignerFolder", "Properties");
            group.AddProperty("RootNamespace", projectName);
            group.AddProperty("AssemblyName", projectName);
            group.AddProperty("TargetFrameworkVersion", "4.5.2");

            var conditionedGroup = root.AddPropertyGroup();
            conditionedGroup.AddProperty("DebugSymbols", "true");
            conditionedGroup.AddProperty("DebugType", "full");
            conditionedGroup.AddProperty("Optimize", "false");
            conditionedGroup.AddProperty("OutputPath", "bin\\Debug\\");
            conditionedGroup.AddProperty("DefineConstants", "DEBUG;TRACE");
            conditionedGroup.AddProperty("ErrorReport", "prompt");
            conditionedGroup.AddProperty("WarningLevel", "4");
            conditionedGroup.Condition = " '$(Configuration)|$(Platform)' == 'Debug|AnyCPU' ";
        }

        private static void AddImports(ProjectRootElement root)
        {
            root.AddImport("$(MSBuildToolsPath)\\Microsoft.CSharp.targets");
            var import = root.AddImport("$(MSBuildExtensionsPath)\\$(MSBuildToolsVersion)\\Microsoft.Common.props");
            import.Condition = "Exists('$(MSBuildExtensionsPath)\\$(MSBuildToolsVersion)\\Microsoft.Common.props')";
        }

        private static ProjectItemGroupElement AddItems(ProjectRootElement elem, string groupName, params string[] items)
        {                   
            var group = elem.AddItemGroup();
            foreach(var item in items)
            {
                group.AddItem(groupName, item);                    
            }
            return group;
        }
    }
}
 