using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SeleniumAutomationGenerator.Generator;
using SeleniumAutomationGenerator;
using SeleniumAutomationGenerator.Models;
using Moq;
using System.IO;
using FluentAssertions;
using SeleniumAutomationGenerator.Generator.PropertyGenerators;

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
            Mock<IComponentAddin> addin = new Mock<IComponentAddin>();
            addin.Setup(add => add.AddinKey).Returns(KEY);
            addin.Setup(add => add.GenerateHelpers(NAME)).Returns(new string[] { "void Main(){}" });
            addin.Setup(add => add.Type).Returns("string");
            BasicComponentsContainer basicComponentsContainer = new BasicComponentsContainer();
            basicComponentsContainer.AddAddin(addin.Object);
            BasicPageGenerator generator = new BasicPageGenerator(basicComponentsContainer, new DriverFindElementPropertyGenerator("Driver"), "Infastructure");
            string classStr = generator.GenerateComponentClass("Foo", new[] { new ElementSelectorData() { FullSelector = "aaa", Name = NAME, Type = KEY } });
            File.WriteAllText("test01.cs", classStr);
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
            var property = propertyGen.CreateNode(addin.Object, NAME, SELECTOR);
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
            var property = propertyGen.CreateNode(addin.Object, NAME, SELECTOR);
            property.Should().Be($"public {TYPE} {NAME} => {DRIVER_PROP_NAME}.FindElement(By.ClassName(\"{SELECTOR}\"));");
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
            var property = propertyGen.CreateNode(addin.Object, NAME, SELECTOR);
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
            var property = propertyGen.CreateNode(addin.Object, NAME, SELECTOR);
            property.Should().Be($"protected {TYPE} {NAME} => new {TYPE}({DRIVER_PROP_NAME},{PARENT_ELEMENT_NAME}.FindElement(By.ClassName(\"{SELECTOR}\")));");
        }
    }
}
