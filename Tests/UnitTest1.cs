﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using SeleniumAutomationGenerator;
using Moq;
using System.IO;
using FluentAssertions;
using SeleniumAutomationGenerator.Generator.PropertyGenerators;
using SeleniumAutomationGenerator.Utils;
using BaseComponentsAddins;
using System.Collections.Generic;
using System.Linq;
using Core;
using Core.Models;
using Core.Utils;
using SeleniumAutomationGenerator.Generator.ComponentsGenerators;
using System;
using SeleniumAutomationGenerator.Builders;

namespace Tests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            const string KEY = "ccc";
            const string NAME = "bbb";
            const string CLASS_NAME = "DishCreator";
            const string SELECTOR = "auto-page-" + CLASS_NAME;
            ComponentsContainer basicComponentsContainer = ComponentsContainer.Instance;
            PageGenerator generator = new PageGenerator(new BasicClassBuilder(), new DriverFindElementPropertyGenerator("Driver"), basicComponentsContainer, Consts.PAGES_NAMESPACE);
            Mock<IComponentAddin> addin = new Mock<IComponentAddin>();
            addin.Setup(add => add.AddinKey).Returns(KEY);
            addin.Setup(add => add.GenerateHelpers(CLASS_NAME, NAME, generator.PropertyGenerator)).Returns(new[] { "void Main(){}", "public void Main2(){}" });
            addin.Setup(add => add.Type).Returns("string");

            basicComponentsContainer.AddAddin(addin.Object);

            var classStr = generator.GenerateComponentClass(SELECTOR, new[] { new ElementSelectorData { FullSelector = "aaa", Name = NAME, Type = KEY, AutomationAttributes = new string[0] } });
            Directory.CreateDirectory(NamespaceFileConverter.ConvertNamespaceToFilePath(Consts.PAGES_NAMESPACE));
            Directory.CreateDirectory(NamespaceFileConverter.ConvertNamespaceToFilePath(Consts.COMPONENTS_NAMESPACE));
            File.WriteAllText(classStr.CsFilePath, classStr.Body);
        }
        [TestMethod]
        public void TestMethod6()
        {
            string file = File.ReadAllText(@"TestFiles\Test1.html");
            const string KEY = "button";
            const string NAME = "Dish";
            const string SECOND_NAME = "CompleteDish";
            const string CLASS_NAME = "DishCreator";
            ComponentsContainer basicComponentsContainer = ComponentsContainer.Instance;
            ComponentsFactory factory = new ComponentsFactory(basicComponentsContainer, basicComponentsContainer, basicComponentsContainer, basicComponentsContainer);
            Mock<IComponentAddin> addin = new Mock<IComponentAddin>();
            addin.Setup(add => add.AddinKey).Returns(KEY);
            addin.Setup(add => add.GenerateHelpers(CLASS_NAME, NAME, It.IsAny<IPropertyGenerator>())).Returns(new[] { $"{CLASS_NAME} With{NAME}(string {NAME.ToLower()}){{}}" });
            addin.Setup(add => add.GenerateHelpers(CLASS_NAME, SECOND_NAME, It.IsAny<IPropertyGenerator>())).Returns(new[] { $"{CLASS_NAME} {SECOND_NAME}(){{}}" });
            addin.Setup(add => add.Type).Returns(Consts.WEB_ELEMENT_CLASS_NAME);

            basicComponentsContainer.AddAddin(addin.Object);

            var files = factory.CreateCsOutput(file);
            Directory.CreateDirectory(NamespaceFileConverter.ConvertNamespaceToFilePath(Consts.PAGES_NAMESPACE));
            Directory.CreateDirectory(NamespaceFileConverter.ConvertNamespaceToFilePath(Consts.COMPONENTS_NAMESPACE));
            foreach (ComponentGeneratorOutput innerFile in files)
            {
                File.WriteAllText(innerFile.CsFilePath, innerFile.Body);
            }
        }

        [TestMethod]
        public void TestMethod7()
        {
            string file = File.ReadAllText(@"TestFiles\Test1.html");
            const string KEY = "label";
            Mock<IComponentAddin> addin = new Mock<IComponentAddin>();
            addin.Setup(add => add.AddinKey).Returns(KEY);
            addin.Setup(add => add.Type).Returns("string");
            ComponentsContainer basicComponentsContainer = ComponentsContainer.Instance;
            basicComponentsContainer.AddAddin(new LabelAddin());
            basicComponentsContainer.AddAddin(new InputAddin());
            ComponentsFactory factory = new ComponentsFactory(basicComponentsContainer, basicComponentsContainer, basicComponentsContainer, basicComponentsContainer);
            var files = factory.CreateCsOutput(file);
            Directory.CreateDirectory(NamespaceFileConverter.ConvertNamespaceToFilePath(Consts.PAGES_NAMESPACE));
            Directory.CreateDirectory(NamespaceFileConverter.ConvertNamespaceToFilePath(Consts.COMPONENTS_NAMESPACE));
            foreach (var innerFile in files)
            {
                File.WriteAllText(innerFile.CsFilePath, innerFile.Body);
            }
        }

        [TestMethod]
        public void TestMethod2()
        {
            const string KEY = "ccc";
            const string NAME = "bbb";
            const string TYPE = "string";
            const string SELECTOR = "aaa";
            const string DRIVER_PROP_NAME = "Driver";

            Mock<IComponentAddin> addin = new Mock<IComponentAddin>();
            addin.Setup(add => add.AddinKey).Returns(KEY);
            addin.Setup(add => add.Type).Returns(TYPE);
            addin.Setup(add => add.IsPropertyModifierPublic).Returns(true);
            addin.Setup(add => add.IsArrayedAddin).Returns(false);
            var propertyGen = new DriverFindElementPropertyGenerator(DRIVER_PROP_NAME);
            var property = propertyGen.CreateProperty(addin.Object, NAME, SELECTOR);
            property.Statement.Should().Be($"public {TYPE} {NAME} => {DRIVER_PROP_NAME}.FindElement(By.ClassName(\"{SELECTOR}\")).Text;");
        }
        [TestMethod]
        public void TestMethod3()
        {
            const string KEY = "ccc";
            const string NAME = "bbb";
            const string TYPE = "IWebElement";
            const string SELECTOR = "aaa";
            const string DRIVER_PROP_NAME = "Driver";

            Mock<IComponentAddin> addin = new Mock<IComponentAddin>();
            addin.Setup(add => add.AddinKey).Returns(KEY);
            addin.Setup(add => add.Type).Returns(TYPE);
            addin.Setup(add => add.IsPropertyModifierPublic).Returns(true);

            var propertyGen = new DriverFindElementPropertyGenerator(DRIVER_PROP_NAME);
            var property = propertyGen.CreateProperty(addin.Object, NAME, SELECTOR);
            property.Statement.Should().Be($"public {TYPE} {NAME}Element => {DRIVER_PROP_NAME}.FindElement(By.ClassName(\"{SELECTOR}\"));");
        }
        [TestMethod]
        public void TestMethod4()
        {
            const string KEY = "ccc";
            const string NAME = "bbb";
            const string TYPE = "CustomClass";
            const string SELECTOR = "aaa";
            const string DRIVER_PROP_NAME = "Driver";
            Mock<IComponentAddin> addin = new Mock<IComponentAddin>();
            addin.Setup(add => add.AddinKey).Returns(KEY);
            addin.Setup(add => add.Type).Returns(TYPE);
            addin.Setup(add => add.CtorContainsDriver).Returns(true);
            var propertyGen = new DriverFindElementPropertyGenerator(DRIVER_PROP_NAME);
            var property = propertyGen.CreateProperty(addin.Object, NAME, SELECTOR);
            property.Statement.Should().Be($"protected {TYPE} {NAME} => new {TYPE}({DRIVER_PROP_NAME},{DRIVER_PROP_NAME}.FindElement(By.ClassName(\"{SELECTOR}\")));");
        }

        [TestMethod]
        public void TestMethod5()
        {
            const string KEY = "ccc";
            const string NAME = "bbb";
            const string TYPE = "CustomClass";
            const string SELECTOR = "aaa";
            const string DRIVER_PROP_NAME = "Driver";
            const string PARENT_ELEMENT_NAME = "_parentElement";
            Mock<IComponentAddin> addin = new Mock<IComponentAddin>();
            addin.Setup(add => add.AddinKey).Returns(KEY);
            addin.Setup(add => add.Type).Returns(TYPE);
            addin.Setup(add => add.CtorContainsDriver).Returns(true);
            var propertyGen = new ParentElementFindElementPropertyGenerator(DRIVER_PROP_NAME, PARENT_ELEMENT_NAME);
            var property = propertyGen.CreateProperty(addin.Object, NAME, SELECTOR);
            property.Statement.Should().Be($"protected {TYPE} {NAME} => new {TYPE}({DRIVER_PROP_NAME},{PARENT_ELEMENT_NAME}.FindElement(By.ClassName(\"{SELECTOR}\")));");
        }

        [TestMethod]
        public void TestMethod8()
        {
            const string KEY = "ccc";
            const string NAME = "bbb";
            const string TYPE = "CustomClass";
            const string SELECTOR = "aaa";
            const string DRIVER_PROP_NAME = "Driver";
            const string PARENT_ELEMENT_NAME = "_parentElement";
            Mock<IComponentAddin> addin = new Mock<IComponentAddin>();
            addin.Setup(add => add.AddinKey).Returns(KEY);
            addin.Setup(add => add.Type).Returns(TYPE);
            addin.Setup(add => add.CtorContainsDriver).Returns(false);

            var propertyGen = new ParentElementFindElementPropertyGenerator(DRIVER_PROP_NAME, PARENT_ELEMENT_NAME);
            var property = propertyGen.CreateProperty(addin.Object, NAME, SELECTOR);
            property.Statement.Should().Be($"protected {TYPE} {NAME} => new {TYPE}({PARENT_ELEMENT_NAME}.FindElement(By.ClassName(\"{SELECTOR}\")));");
        }
        [TestMethod]
        public void TestMethod9()
        {
            const string KEY = "ccc";
            const string NAME = "bbb";
            const string TYPE = "CustomClass";
            const string SELECTOR = "aaa";
            const string DRIVER_PROP_NAME = "Driver";
            const string PARENT_ELEMENT_NAME = "_parentElement";
            Mock<IComponentAddin> addin = new Mock<IComponentAddin>();
            addin.Setup(add => add.AddinKey).Returns(KEY);
            addin.Setup(add => add.Type).Returns(TYPE);
            addin.Setup(add => add.CtorContainsDriver).Returns(false);
            addin.Setup(add => add.IsArrayedAddin).Returns(true);

            var propertyGen = new ParentElementFindElementPropertyGenerator(DRIVER_PROP_NAME, PARENT_ELEMENT_NAME);
            var property = propertyGen.CreateProperty(addin.Object, NAME, SELECTOR);
            property.Statement.Should().Be($"protected {TYPE}[] {NAME} => {PARENT_ELEMENT_NAME}.FindElements(By.ClassName(\"{SELECTOR}\")).Select(elm=> new {TYPE}(elm)).ToArray();");
        }
        [TestMethod]
        public void TestMethod10()
        {
            const string KEY = "ccc";
            const string NAME = "bbb";
            const string TYPE = "CustomClass";
            const string SELECTOR = "aaa";
            const string DRIVER_PROP_NAME = "Driver";
            const string PARENT_ELEMENT_NAME = "_parentElement";
            Mock<IComponentAddin> addin = new Mock<IComponentAddin>();
            addin.Setup(add => add.AddinKey).Returns(KEY);
            addin.Setup(add => add.Type).Returns(TYPE);
            addin.Setup(add => add.CtorContainsDriver).Returns(true);
            addin.Setup(add => add.IsArrayedAddin).Returns(true);

            var propertyGen = new ParentElementFindElementPropertyGenerator(DRIVER_PROP_NAME, PARENT_ELEMENT_NAME);
            var property = propertyGen.CreateProperty(addin.Object, NAME, SELECTOR);
            property.Statement.Should().Be($"protected {TYPE}[] {NAME} => {PARENT_ELEMENT_NAME}.FindElements(By.ClassName(\"{SELECTOR}\")).Select(elm=> new {TYPE}({DRIVER_PROP_NAME},elm)).ToArray();");
        }

        [TestMethod]
        public void TestMethod11()
        {
            const string KEY = "ccc";
            const string NAME = "bbb";
            const string TYPE = "string";
            const string SELECTOR = "aaa";
            const string DRIVER_PROP_NAME = "Driver";
            const string PARENT_ELEMENT_NAME = "_parentElement";
            Mock<IComponentAddin> addin = new Mock<IComponentAddin>();
            addin.Setup(add => add.AddinKey).Returns(KEY);
            addin.Setup(add => add.Type).Returns(TYPE);
            addin.Setup(add => add.CtorContainsDriver).Returns(true);
            addin.Setup(add => add.IsArrayedAddin).Returns(true);

            var propertyGen = new ParentElementFindElementPropertyGenerator(DRIVER_PROP_NAME, PARENT_ELEMENT_NAME);
            var property = propertyGen.CreateProperty(addin.Object, NAME, SELECTOR);
            property.Statement.Should().Be($"protected {TYPE}[] {NAME} => {PARENT_ELEMENT_NAME}.FindElements(By.ClassName(\"{SELECTOR}\")).Select(elm=> elm.Text).ToArray();");
        }

        [TestMethod]
        public void TestMethod12()
        {
            List<ComponentGeneratorOutput>[] expectedOutpus = {
                new List<ComponentGeneratorOutput> {new ComponentGeneratorOutput { Body="a" ,CsFilePath="b"} },
                new List<ComponentGeneratorOutput> {new ComponentGeneratorOutput { Body="c" ,CsFilePath="d"} },
                new List<ComponentGeneratorOutput> {new ComponentGeneratorOutput { Body="e" ,CsFilePath="f"} }
            };
            const string DIRECTORY = "hello";
            Mock<IHtmlsFinder> finder = new Mock<IHtmlsFinder>();
            Mock<IComponentsFactory> factory = new Mock<IComponentsFactory>();
            finder.Setup(f => f.GetFilesTexts(DIRECTORY)).Returns(new[] { "my", "new", "world" });
            factory.Setup(f => f.CreateCsOutput("my")).Returns(expectedOutpus[0]);
            factory.Setup(f => f.CreateCsOutput("new")).Returns(expectedOutpus[1]);
            factory.Setup(f => f.CreateCsOutput("world")).Returns(expectedOutpus[2]);
            WebFolderToCsFilesConverter converter = new WebFolderToCsFilesConverter(factory.Object, finder.Object);
            List<ComponentGeneratorOutput> outputs = converter.GenerateClasses(DIRECTORY);
            outputs.Should().BeEquivalentTo(expectedOutpus.SelectMany(a => a));
        }
        [TestMethod]
        public void TestMethod13()
        {
            List<ComponentGeneratorOutput>[] expectedOutpus = {
                new List<ComponentGeneratorOutput> {new ComponentGeneratorOutput { Body="a" ,CsFilePath="b"} },
                new List<ComponentGeneratorOutput> {new ComponentGeneratorOutput { Body="z" ,CsFilePath="b"} },
                new List<ComponentGeneratorOutput> {new ComponentGeneratorOutput { Body="a" ,CsFilePath="c"} },
                new List<ComponentGeneratorOutput> {new ComponentGeneratorOutput { Body="d" ,CsFilePath="e"} },
                new List<ComponentGeneratorOutput> {new ComponentGeneratorOutput { Body="f" ,CsFilePath="g"} }
            };
            const string DIRECTORY = "hello";
            Mock<IHtmlsFinder> finder = new Mock<IHtmlsFinder>();
            Mock<IComponentsFactory> factory = new Mock<IComponentsFactory>();
            finder.Setup(f => f.GetFilesTexts(DIRECTORY)).Returns(new[] { "my", "new", "world", "was", "spinning" });
            factory.Setup(f => f.CreateCsOutput("my")).Returns(expectedOutpus[0]);
            factory.Setup(f => f.CreateCsOutput("new")).Returns(expectedOutpus[1]);
            factory.Setup(f => f.CreateCsOutput("world")).Returns(expectedOutpus[2]);
            factory.Setup(f => f.CreateCsOutput("was")).Returns(expectedOutpus[3]);
            factory.Setup(f => f.CreateCsOutput("spinning")).Returns(expectedOutpus[3]);
            WebFolderToCsFilesConverter converter = new WebFolderToCsFilesConverter(factory.Object, finder.Object);
            List<ComponentGeneratorOutput> outputs = converter.GenerateClasses(DIRECTORY);
            outputs.Should().HaveCount(expectedOutpus.Length - 1);
        }
        [TestMethod]
        public void TestMethod14()
        {
            string file = File.ReadAllText(@"TestFiles\Test1.html");
            BuiltInComponentsInserter.InsertBuiltInComponents();
            var basicComponentsContainer = ComponentsContainer.Instance;
            ComponentsFactory factory = new ComponentsFactory(basicComponentsContainer, basicComponentsContainer, basicComponentsContainer, basicComponentsContainer);
            var files = factory.CreateCsOutput(file);
            Directory.CreateDirectory(NamespaceFileConverter.ConvertNamespaceToFilePath(Consts.PAGES_NAMESPACE));
            Directory.CreateDirectory(NamespaceFileConverter.ConvertNamespaceToFilePath(Consts.COMPONENTS_NAMESPACE));
            foreach (var innerFile in files)
            {
                File.WriteAllText(innerFile.CsFilePath, innerFile.Body);
            }
        }
        [TestMethod]
        public void TestMethod15()
        {
            ProjectGenerator generator = new ProjectGenerator();
            generator.GenerateProject(Environment.CurrentDirectory, Consts.INFASTRUCTURE_NAMESPACE, "Automation");
        }
    }
}
