namespace SeleniumAutomationGenerator
{
    public interface IAddinsContainer
    {        
        void AddAddin(IComponentAddin newAddin, bool setAsDefault = false);

        IComponentAddin GetAddin(string key);
    }
}
