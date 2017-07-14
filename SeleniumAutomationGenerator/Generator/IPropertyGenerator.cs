namespace SeleniumAutomationGenerator
{
    public interface IPropertyGenerator
    {
        string CreateProperty(IComponentAddin type , string propName, string selector);
    }   
}