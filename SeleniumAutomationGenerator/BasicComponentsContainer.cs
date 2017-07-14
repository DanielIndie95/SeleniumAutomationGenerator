using System.Collections.Generic;
using System.Linq;

namespace SeleniumAutomationGenerator
{
    public class BasicComponentsContainer : IAddinsContainer
    {
        private IComponentAddin _defaultAddin;
        private Dictionary<string, IComponentAddin> _addins;

        public IComponentAddin[] Addins => _addins.Values.ToArray();
        public BasicComponentsContainer()
        {
            _addins = new Dictionary<string, IComponentAddin>();
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
