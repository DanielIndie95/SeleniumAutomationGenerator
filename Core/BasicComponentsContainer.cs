using System.Collections.Generic;
using System.Linq;

namespace Core
{
    public sealed class ComponentsContainer
    {
        public static ComponentsContainer Instance { get; }

        private IComponentAddin _defaultAddin;
        private readonly Dictionary<string, IComponentAddin> _addins;

        public IComponentAddin[] Addins => _addins.Values.ToArray();
        private ComponentsContainer()
        {
            _addins = new Dictionary<string, IComponentAddin>();
        }

        static ComponentsContainer()
        {
            Instance = new ComponentsContainer();
        }

        public void AddAddin(IComponentAddin newAddin, bool setAsDefault = false)
        {
            if (setAsDefault)
                _defaultAddin = newAddin;

            _addins[newAddin.AddinKey] = newAddin;
        }

        public IComponentAddin GetAddin(string key)
        {
            IComponentAddin addin = _addins.TryGetValue(key, out IComponentAddin tryAddin)
                ? tryAddin : _defaultAddin;
            return addin;
        }
    }
}
