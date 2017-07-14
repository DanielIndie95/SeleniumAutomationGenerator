using System.Collections.Generic;
using System.Linq;

namespace SeleniumAutomationGenerator
{
    public sealed class ComponentsContainer
    {
        private static ComponentsContainer _value;
        public static ComponentsContainer Instance => _value;

        private IComponentAddin _defaultAddin;
        private Dictionary<string, IComponentAddin> _addins;

        public IComponentAddin[] Addins => _addins.Values.ToArray();
        private ComponentsContainer()
        {
            _addins = new Dictionary<string, IComponentAddin>();
        }

        static ComponentsContainer()
        {
            _value = new ComponentsContainer();
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
