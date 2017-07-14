using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SeleniumAutomationGenerator.Generator;
using SeleniumAutomationGenerator;
using SeleniumAutomationGenerator.Models;
using Moq;
using System.IO;
using FluentAssertions;

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
            BasicPageGenerator generator = new BasicPageGenerator(basicComponentsContainer, new DriverFindElementPropertyGenerator(), "Infastructure");
            string classStr = generator.GeneratePageClass("Foo", new[] { new ElementSelectorData() { FullSelector = "aaa", Name = NAME, Type = KEY } });
            int a = 5;
            File.WriteAllText("test01.cs",classStr);
        }

        [TestMethod]
        public void TestMethod2()
        {
            const string KEY = "ccc";
            const string NAME = "bbb";
            const string TYPE = "string";
            const string SELECTOR = "aaa";
            Mock<IComponentAddin> addin = new Mock<IComponentAddin>();
            addin.Setup(add => add.AddinKey).Returns(KEY);
            addin.Setup(add => add.Type).Returns(TYPE);
            addin.Setup(add => add.IsPropertyModifierPublic).Returns(true);
            var propertyGen = new DriverFindElementPropertyGenerator();
            var property = propertyGen.CreateNode(addin.Object, NAME, SELECTOR);
            property.Should().Be($"public {TYPE} {NAME} => Driver.FindElement(By.ClassName(\"{SELECTOR}\")).Text;");
            int a = 5;
        }
        [TestMethod]
        public void TestMethod3()
        {
            const string KEY = "ccc";
            const string NAME = "bbb";
            const string TYPE = "IWebDriver";
            const string SELECTOR = "aaa";
            Mock<IComponentAddin> addin = new Mock<IComponentAddin>();
            addin.Setup(add => add.AddinKey).Returns(KEY);
            addin.Setup(add => add.Type).Returns(TYPE);
            addin.Setup(add => add.IsPropertyModifierPublic).Returns(true);

            var propertyGen = new DriverFindElementPropertyGenerator();
            var property = propertyGen.CreateNode(addin.Object, NAME, SELECTOR);
            property.Should().Be($"public {TYPE} {NAME} => Driver.FindElement(By.ClassName(\"{SELECTOR}\"));");
            int a = 5;
        }
        [TestMethod]
        public void TestMethod4()
        {
            const string KEY = "ccc";
            const string NAME = "bbb";
            const string TYPE = "CustomClass";
            const string SELECTOR = "aaa";
            Mock<IComponentAddin> addin = new Mock<IComponentAddin>();
            addin.Setup(add => add.AddinKey).Returns(KEY);
            addin.Setup(add => add.Type).Returns(TYPE);
            var propertyGen = new DriverFindElementPropertyGenerator();
            var property = propertyGen.CreateNode(addin.Object, NAME, SELECTOR);
            property.Should().Be($"protected {TYPE} {NAME} => new {TYPE}(Driver,Driver.FindElement(By.ClassName(\"{SELECTOR}\")));");
            int a = 5;
        }
    }
}
