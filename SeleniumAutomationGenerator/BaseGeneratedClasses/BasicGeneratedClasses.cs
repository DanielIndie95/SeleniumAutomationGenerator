
using Core.Models;
using Core.Utils;
using SeleniumAutomationGenerator.Utils;

namespace SeleniumAutomationGenerator.BaseGeneratedClasses
{
    public class BasicGeneratedClasses
    {

        public static ComponentGeneratorOutput DriverContainer
        {
            get
            {
                string classBody = $@"using OpenQA.Selenium; 
namespace {Consts.BASE_NAMESPACE}
{{
    public class DriverContainer
    {{       
        protected IWebDriver Driver {{ get; }}

        public DriverContainer(IWebDriver driver)
        {{
            Driver = driver;
        }}
        public DriverContainer(DriverContainer driver): this(driver.Driver)
        {{
            
        }}
    }}
}}";
                return CreateBasicClass(classBody,"DriverContainer");
            }
        }
        
        private static ComponentGeneratorOutput CreateBasicClass(string body,string className)
        {
            string fileName = NamespaceFileConverter.ConvertNamespaceToFilePath(Consts.BASE_NAMESPACE, $"{className}.cs");
            return new ComponentGeneratorOutput()
            {
                Body = body,
                CsFilePath = fileName
            };
        }
    }
}
