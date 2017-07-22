namespace Core
{
    public interface IAddinContainer
    {
        void AddAddin(IComponentAddin newAddin);
        IComponentAddin GetAddin(string key);
        bool ContainsAddin(string key);
    }
}
