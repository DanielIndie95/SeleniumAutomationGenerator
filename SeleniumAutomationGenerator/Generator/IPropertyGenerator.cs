namespace SeleniumAutomationGenerator
{
    public interface IPropertyGenerator
    {
        string CreateNode(IComponentAddin type , string propName, string selector);
        string CreateNodeAsList(string type, string propName, string selector);
    }   
}