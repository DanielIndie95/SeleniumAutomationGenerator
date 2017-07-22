namespace Core
{
    public interface IAddinContainer
    {
        void AddAddin(IComponentAddin newAddin, bool setAsDefault = false);
        IComponentAddin GetAddin(string key);
    }
}
