namespace Core
{
    public interface IElementAttribute
    {
        string[] GetProperties(string webElementPropertyName);
        string[] GetMethods(string webElementPropertyName);
        string Name { get; }
    }
}