using System.Collections.Generic;
using System.Linq;

namespace SeleniumAutomationGenerator
{
    public class BasicCompoenentsContainer : IComponentsContainer
    {
        private IComponentAddin _defaultAddin;
        private Dictionary<string, IComponentAddin> _addins;

        public IComponentAddin[] Addins => _addins.Values.ToArray();
        public BasicCompoenentsContainer()
        {
            _addins = new Dictionary<string, IComponentAddin>();
        }

        public void AddAddin(IComponentAddin newAddin, bool setAsDefault = false)
        {
            if (setAsDefault)
                _defaultAddin = newAddin;
            
            _addins.Add(newAddin.AddinKey, newAddin);
        }

        public IComponentAddin GetAddin(string key)
        {
            IComponentAddin addin = _addins.TryGetValue(key, out IComponentAddin tryAddin)
                ? tryAddin : _defaultAddin;
            return addin;
        }             
    }
}
