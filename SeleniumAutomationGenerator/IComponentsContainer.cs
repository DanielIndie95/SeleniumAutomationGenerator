using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace SeleniumAutomationGenerator
{
    public interface IComponentsContainer
    {        
        void AddAddin(IComponentAddin newAddin, bool setAsDefault = false);

        IComponentAddin GetAddin(string key);
    }
}
