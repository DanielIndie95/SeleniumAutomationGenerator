using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SeleniumAutomationGenerator.Generator;
using SeleniumAutomationGenerator;
using SeleniumAutomationGenerator.Models;
using Moq;
using System.IO;
using FluentAssertions;
using SeleniumAutomationGenerator.Generator.PropertyGenerators;
using SeleniumAutomationGenerator.Utils;

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

            Mock<IComponentAddin> addin = new Mock<IComponentAddin>();
            addin.Setup(add => add.AddinKey).Returns(KEY);
            addin.Setup(add => add.GenerateHelpers(CLASS_NAME, NAME)).Returns(new string[] { "void Main(){}", "public void Main2(){}" });
            addin.Setup(add => add.Type).Returns("string");
            BasicComponentsContainer basicComponentsContainer = new BasicComponentsContainer();
            basicComponentsContainer.AddAddin(addin.Object);
            BasicPageGenerator generator = new BasicPageGenerator(basicComponentsContainer, new DriverFindElementPropertyGenerator("Driver"), Consts.PAGES_NAMESPACE);
            var classStr = generator.GenerateComponentClass(CLASS_NAME, new[] { new ElementSelectorData() { FullSelector = "aaa", Name = NAME, Type = KEY } });
            Directory.CreateDirectory(Consts.PAGES_NAMESPACE);
            Directory.CreateDirectory(Consts.COMPONENTS_NAMESPACE);
            File.WriteAllText(classStr.CsFileName, classStr.Body);
        }
        [TestMethod]
        public void TestMethod6()
        {
            string file = File.ReadAllText(@"TestFiles\create-dish.html");
            const string KEY = "button";
            const string NAME = "Dish";
            const string SECOND_NAME = "CompleteDish";
            const string CLASS_NAME = "DishCreator";
            Mock<IComponentAddin> addin = new Mock<IComponentAddin>();
            addin.Setup(add => add.AddinKey).Returns(KEY);
            addin.Setup(add => add.GenerateHelpers(CLASS_NAME, NAME)).Returns(new string[] { $"{CLASS_NAME} With{NAME}(string {NAME.ToLower()}){{}}" });
            addin.Setup(add => add.GenerateHelpers(CLASS_NAME, SECOND_NAME)).Returns(new string[] { $"{CLASS_NAME} {SECOND_NAME}(){{}}" });
            addin.Setup(add => add.Type).Returns(Consts.WEB_ELEMENT_CLASS_NAME);
            BasicComponentsContainer basicComponentsContainer = new BasicComponentsContainer();
            basicComponentsContainer.AddAddin(addin.Object);
            ComponentsFactory factory = new ComponentsFactory(basicComponentsContainer);
            var files = factory.CreateCsOutput(file);
            Directory.CreateDirectory(NamespaceFileConverter.ConvertNamespaceToFilePath(Consts.PAGES_NAMESPACE));
            Directory.CreateDirectory(NamespaceFileConverter.ConvertNamespaceToFilePath(Consts.COMPONENTS_NAMESPACE));
            foreach (var innerFile in files)
            {
                File.WriteAllText(innerFile.CsFileName, innerFile.Body);
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
            BasicComponentsContainer basicComponentsContainer = new BasicComponentsContainer();
            basicComponentsContainer.AddAddin(addin.Object);
            ComponentsFactory factory = new ComponentsFactory(basicComponentsContainer);
            var files = factory.CreateCsOutput(file);
            Directory.CreateDirectory(NamespaceFileConverter.ConvertNamespaceToFilePath(Consts.PAGES_NAMESPACE));
            Directory.CreateDirectory(NamespaceFileConverter.ConvertNamespaceToFilePath(Consts.COMPONENTS_NAMESPACE));
            foreach (var innerFile in files)
            {
                File.WriteAllText(innerFile.CsFileName, innerFile.Body);
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
            var propertyGen = new DriverFindElementPropertyGenerator(DRIVER_PROP_NAME);
            var property = propertyGen.CreateProperty(addin.Object, NAME, SELECTOR);
            property.Should().Be($"public {TYPE} {NAME} => {DRIVER_PROP_NAME}.FindElement(By.ClassName(\"{SELECTOR}\")).Text;");
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
            property.Should().Be($"public {TYPE} {NAME}Element => {DRIVER_PROP_NAME}.FindElement(By.ClassName(\"{SELECTOR}\"));");
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
            var propertyGen = new DriverFindElementPropertyGenerator(DRIVER_PROP_NAME);
            var property = propertyGen.CreateProperty(addin.Object, NAME, SELECTOR);
            property.Should().Be($"protected {TYPE} {NAME} => new {TYPE}({DRIVER_PROP_NAME},{DRIVER_PROP_NAME}.FindElement(By.ClassName(\"{SELECTOR}\")));");
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

            var propertyGen = new ParentElementFindElementPropertyGenerator(DRIVER_PROP_NAME, PARENT_ELEMENT_NAME);
            var property = propertyGen.CreateProperty(addin.Object, NAME, SELECTOR);
            property.Should().Be($"protected {TYPE} {NAME} => new {TYPE}({DRIVER_PROP_NAME},{PARENT_ELEMENT_NAME}.FindElement(By.ClassName(\"{SELECTOR}\")));");
        }        
    }
}
