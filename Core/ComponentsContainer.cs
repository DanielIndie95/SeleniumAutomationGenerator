using System.Collections.Generic;

namespace Core
{
    public sealed class ComponentsContainer : IElementAttributeContainer, IAddinContainer, IFileCreatorContainer, IClassAppenderContainer
    {
        public static ComponentsContainer Instance { get; }

        private IComponentFileCreator _defaultFileCreator;

        private readonly Dictionary<string, IComponentAddin> _addins;
        private readonly Dictionary<string, IElementAttribute> _attributes;
        private readonly Dictionary<string, IComponentFileCreator> _fileCreators;
        private readonly Dictionary<string, IComponentClassAppender> _classAppenders;

        private ComponentsContainer()
        {
            _addins = new Dictionary<string, IComponentAddin>();
            _attributes = new Dictionary<string, IElementAttribute>();
            _fileCreators = new Dictionary<string, IComponentFileCreator>();
            _classAppenders = new Dictionary<string, IComponentClassAppender>();
        }

        static ComponentsContainer()
        {
            Instance = new ComponentsContainer();
        }

        public void AddAddin(IComponentAddin newAddin)
        {
            _addins[newAddin.AddinKey] = newAddin;
        }

        public IComponentAddin GetAddin(string key)
        {
            IComponentAddin addin = _addins.TryGetValue(key, out IComponentAddin tryAddin)
                ? tryAddin
                : null;
            return addin;
        }

        public void AddCustomAttribute(IElementAttribute attribute)
        {
            _attributes[attribute.Identifier] = attribute;
        }

        public IElementAttribute GetElementAttribute(string attribute)
        {
            return _attributes.TryGetValue(attribute, out IElementAttribute att) ? att : null;
        }

        public void AddFileCreatorComponent(string key, IComponentFileCreator newComponentFileCreator, bool setAsDefault = false)
        {
            _fileCreators[key] = newComponentFileCreator;
            if (setAsDefault)
                _defaultFileCreator = newComponentFileCreator;
        }

        public IComponentFileCreator GetFileCreator(string componentKey)
        {
            return _fileCreators.TryGetValue(componentKey, out IComponentFileCreator creator)
                ? creator : _defaultFileCreator;
        }

        public void AddComponentTypeAppenders(IComponentClassAppender classAppender)
        {
            _classAppenders[classAppender.Identifier] = classAppender;            
        }

        public IComponentClassAppender GetAppender(string appenderIdentifier)
        {
            return _classAppenders.TryGetValue(appenderIdentifier, out IComponentClassAppender appender)
                            ? appender : null;
        }

        public bool ContainsAppender(string appenderIdentifier)
        {
            return GetAppender(appenderIdentifier) != null;
        }

        public bool ContainsCustomAttribute(string attribute)
        {
            return GetElementAttribute(attribute) != null;
        }

        public bool ContainsAddin(string key)
        {
            return GetAddin(key) != null;
        }
    }
}