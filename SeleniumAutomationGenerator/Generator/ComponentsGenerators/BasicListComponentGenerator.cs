using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
