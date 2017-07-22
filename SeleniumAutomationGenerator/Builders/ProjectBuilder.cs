using Core.Models;
using Microsoft.Build.Construction;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SeleniumAutomationGenerator.Builders
{
    public class ProjectBuilder
    {
        public void BuildProject(string projectName, IEnumerable<ComponentGeneratorOutput> files, string csprojDir, string packageConfigFilename)
        {
            ProjectRootElement root = ProjectRootElement.Create();
            AddProperties(projectName, root);

            // references           
            AddItems(root, "Reference", "System", "System.Core", "System.Drawing", "System.Xml.Linq", "System.Data.DataSetExtensions", "Microsoft.CSharp", "System.Data", "System.Net.Http", "System.Xml");
            // items to compile
            AddItems(root, "Compile", files.Select(file => file.CsFilePath).ToArray());

            AddItems(root, "None", packageConfigFilename);
            ProjectTargetElement target = root.AddTarget("Build");
            ProjectTaskElement task = target.AddTask("Csc");
            task.SetParameter("Sources", "@(Compile)");
            task.SetParameter("OutputAssembly", $"{projectName}.dll");
            AddImports(root);

            root.Save($"{csprojDir}\\{projectName}.csproj");
        }

        private static void AddProperties(string projectName, ProjectRootElement root)
        {
            ProjectPropertyGroupElement group = root.AddPropertyGroup();
            group.AddProperty("Configuration", "Debug");
            group.AddProperty("Platform", "AnyCPU");
            group.AddProperty("ProjectGuid", Guid.NewGuid().ToString());
            group.AddProperty("OutputType", "Library");
            group.AddProperty("AppDesignerFolder", "Properties");
            group.AddProperty("RootNamespace", projectName);
            group.AddProperty("AssemblyName", projectName);
            group.AddProperty("TargetFrameworkVersion", "4.5.2");

            ProjectPropertyGroupElement conditionedGroup = root.AddPropertyGroup();
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
            ProjectImportElement import = root.AddImport("$(MSBuildExtensionsPath)\\$(MSBuildToolsVersion)\\Microsoft.Common.props");
            import.Condition = "Exists('$(MSBuildExtensionsPath)\\$(MSBuildToolsVersion)\\Microsoft.Common.props')";
        }

        private static void AddItems(ProjectRootElement elem, string groupName, params string[] items)
        {
            ProjectItemGroupElement group = elem.AddItemGroup();
            foreach (string item in items)
            {
                group.AddItem(groupName, item);
            }
        }
    }
}
