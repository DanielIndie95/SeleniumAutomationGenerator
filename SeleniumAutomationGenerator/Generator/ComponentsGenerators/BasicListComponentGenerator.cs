using System;
using Core;

namespace SeleniumAutomationGenerator.Generator.ComponentsGenerators
{
    public class BasicListComponentGenerator : BasicClassGenerator
    {
        public BasicListComponentGenerator(ComponentsContainer container, IPropertyGenerator propertyGenerator, string namespaceName) : base(container, propertyGenerator, namespaceName)
        {
        }

        protected override string CreateCtor(string className)
        {
            throw new NotImplementedException();
        }

        
    }
}
