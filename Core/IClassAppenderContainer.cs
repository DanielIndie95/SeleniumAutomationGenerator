namespace Core
{
    public interface IClassAppenderContainer
    {
        void AddComponentTypeAppenders(IComponentClassAppender classAppender);
        IComponentClassAppender GetAppender(string appenderIdentifier);
        bool ContainsAppender(string appenderIdentifier);
    }
}
