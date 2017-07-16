namespace Core
{
    public interface IPropertyGenerator
    {
        string CreateProperty(IComponentAddin type , string propName, string selector);
        string GetPropertyName(IComponentAddin addin, string propName);
    }   
}