﻿using SeleniumAutomationGenerator.Generator;
using SeleniumAutomationGenerator.Generator.ClassAppenders;
using SeleniumAutomationGenerator.Generator.PropertyGenerators;
using SeleniumAutomationGenerator.Models;
using SeleniumAutomationGenerator.Utils;
using System.Collections.Generic;
using System.Linq;

namespace SeleniumAutomationGenerator
{
    public class ComponentsFactory
    {
        Dictionary<string, IComponentFileCreator> _fileCreators;
        Dictionary<string, IComponentClassAppender> _classAppenders;

        IComponentFileCreator _defaultFileCreator;
        public ComponentsFactory(ComponentsContainer container)
        {
            _fileCreators = new Dictionary<string, IComponentFileCreator>();
            _classAppenders = new Dictionary<string, IComponentClassAppender>();
            AddComponentClassGeneratorKey("page", new BasicPageGenerator(container, new DriverFindElementPropertyGenerator(Consts.DRIVER_FIELD_NAME), Consts.PAGES_NAMESPACE));
            AddComponentClassGeneratorKey("model", new BasicModelGenerator(container, new ParentElementFindElementPropertyGenerator(Consts.DRIVER_FIELD_NAME, Consts.PARENT_ELEMENT_FIELD_NAME), Consts.PAGES_NAMESPACE, Consts.PARENT_ELEMENT_FIELD_NAME));

            _defaultFileCreator = new BasicComponentGenerator(container, new ParentElementFindElementPropertyGenerator(Consts.DRIVER_FIELD_NAME, Consts.PARENT_ELEMENT_FIELD_NAME), Consts.PAGES_NAMESPACE, Consts.PARENT_ELEMENT_FIELD_NAME);

            AddComponentTypeAppenders("list", new ListClassAppender());            
        }

        public void AddComponentClassGeneratorKey(string key, IComponentFileCreator newComponentFileCreator)
        {
            _fileCreators[key] = newComponentFileCreator;
        }

        public void AddComponentTypeAppenders(string type, IComponentClassAppender classAppender)
        {
            _classAppenders[type] = classAppender;
            _defaultFileCreator.AddExceptionPropertyType(type);
            foreach (var fileCreator in _fileCreators)
            {
                fileCreator.Value.AddExceptionPropertyType(type);
            }
        }
        public IEnumerable<ComponentGeneratorOutput> CreateCsOutput(string body)
        {
            IEnumerable<AutoElementData> children = AutoElementFinder.GetChildren(body);
            IEnumerable<ComponentGeneratorOutput> totalChildren = new List<ComponentGeneratorOutput>();
            foreach (var child in children)
            {
                totalChildren = totalChildren.Union(CreateCsOutput(child.Selector, child.InnerChildrens), new ComponentOutputComparer());
            }
            return totalChildren;
        }

        private IEnumerable<ComponentGeneratorOutput> CreateCsOutput(string selector, IEnumerable<AutoElementData> children, IComponentFileCreator parentClassCreator = null)
        {
            var keyWord = SelectorUtils.GetKeyWordFromSelector(selector);
            if (children.Count() == 0) //not a new cs file
                return new List<ComponentGeneratorOutput>();

            IEnumerable<ElementSelectorData> childrenData = children.Select(ConvertToElementSelectorData);
            if (_classAppenders.ContainsKey(keyWord) && parentClassCreator != null)
            {
                HandleClassAppenders(selector, parentClassCreator, keyWord, childrenData);
            }
            return GetFileCreatorsOutput(selector, children, keyWord, childrenData);
        }

        private void HandleClassAppenders(string selector, IComponentFileCreator parentClassCreator, string keyWord, IEnumerable<ElementSelectorData> childrenData)
        {
            _classAppenders[keyWord].AppendToClass(parentClassCreator, selector, childrenData.ToArray());
        }

        private IEnumerable<ComponentGeneratorOutput> GetFileCreatorsOutput(string selector, IEnumerable<AutoElementData> children, string keyWord, IEnumerable<ElementSelectorData> childrenData)
        {
            IEnumerable<ComponentGeneratorOutput> outputs = new List<ComponentGeneratorOutput>();
            IComponentFileCreator parent = _fileCreators.ContainsKey(keyWord) ? _fileCreators[keyWord] : _defaultFileCreator;
            foreach (var child in children)
            {
                outputs = outputs.Union(CreateCsOutput(child.Selector, child.InnerChildrens.ToList(), parent), new ComponentOutputComparer());
            }
            ComponentGeneratorOutput parentOutput = parent.GenerateComponentClass(selector, childrenData.ToArray());            
            outputs = outputs.Union(new[] { parentOutput }, new ComponentOutputComparer());
            return outputs;
        }

        private ElementSelectorData ConvertToElementSelectorData(AutoElementData data)
        {
            return new ElementSelectorData()
            {
                FullSelector = data.Selector,
                Name = SelectorUtils.GetClassNameFromSelector(data.Selector),
                Type = SelectorUtils.GetKeyWordFromSelector(data.Selector)
            };
        }
    }
}
