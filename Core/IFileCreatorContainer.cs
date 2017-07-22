namespace Core
{
    public interface IFileCreatorContainer
    {
        void AddFileCreatorComponent(string key, IComponentFileCreator newComponentFileCreator, bool setAsDefault = false);
        IComponentFileCreator GetFileCreator(string componentKey);
    }
}
