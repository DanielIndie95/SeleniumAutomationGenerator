namespace Core
{
    public interface IElementAttributeContainer
    {
        void AddCustomAttribute(IElementAttribute attribute);
        IElementAttribute GetElementAttribute(string attribute);
        bool ContainsCustomAttribute(string attribute);
    }
}
