using System.Collections.Generic;
using System.Linq;

namespace Core
{
    public sealed class ComponentsContainer
    {
        public static ComponentsContainer Instance { get; }

        private IComponentAddin _defaultAddin;
        private readonly Dictionary<string, IComponentAddin> _addins;
        private readonly Dictionary<string, IElementAttribute> _attributes;

        private ComponentsContainer()
        {
            _addins = new Dictionary<string, IComponentAddin>();
            _attributes = new Dictionary<string, IElementAttribute>();
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

        public void AddCustomAttribute(IElementAttribute attribute)
        {
            _attributes[attribute.Name] = attribute;
        }

        public IElementAttribute GetElementAttribute(string attribute)
        {
            return _attributes.TryGetValue(attribute, out IElementAttribute att) ? att : null;
        }

        public IComponentAddin GetAddin(string key)
        {
            IComponentAddin addin = _addins.TryGetValue(key, out IComponentAddin tryAddin)
                ? tryAddin
                : _defaultAddin;
            return addin;
        }
    }
}