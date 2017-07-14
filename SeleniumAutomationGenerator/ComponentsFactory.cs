using SeleniumAutomationGenerator.Generator;
using SeleniumAutomationGenerator.Generator.PropertyGenerators;
using SeleniumAutomationGenerator.Models;
using SeleniumAutomationGenerator.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SeleniumAutomationGenerator
{
    public class ComponentsFactory
    {
        Dictionary<string, Func<string, ElementSelectorData[], ComponentGeneratorOutput>> fileCreators;
        Func<string, ElementSelectorData[], ComponentGeneratorOutput> _defaultFileCreator;
        public ComponentsFactory(IAddinsContainer container)
        {
            fileCreators = new Dictionary<string, Func<string, ElementSelectorData[], ComponentGeneratorOutput>>();
            AddComponentGeneratorKey("page", (selector, elements) =>
                                            new BasicPageGenerator(container, new DriverFindElementPropertyGenerator(Consts.DRIVER_FIELD_NAME), Consts.PAGES_NAMESPACE)
                                               .GenerateComponentClass(GetClassNameFromSelector(selector), elements));

            AddComponentGeneratorKey("model", (selector, elements) =>
                                            new BasicModelGenerator(container, new ParentElementFindElementPropertyGenerator(Consts.DRIVER_FIELD_NAME, Consts.PARENT_ELEMENT_FIELD_NAME), Consts.PAGES_NAMESPACE, Consts.PARENT_ELEMENT_FIELD_NAME)
                                               .GenerateComponentClass(GetClassNameFromSelector(selector), elements));

            _defaultFileCreator = (selector, elements) =>
                                           new BasicComponentGenerator(container, new ParentElementFindElementPropertyGenerator(Consts.DRIVER_FIELD_NAME, Consts.PARENT_ELEMENT_FIELD_NAME), Consts.PAGES_NAMESPACE, Consts.PARENT_ELEMENT_FIELD_NAME)
                                              .GenerateComponentClass(GetClassNameFromSelector(selector), elements);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="componentCreator">first arg - selector , second arg - inner Elements , third arg - output</param>
        public void AddComponentGeneratorKey(string key, Func<string, ElementSelectorData[], ComponentGeneratorOutput> componentCreator)
        {
            fileCreators[key] = componentCreator;
        }
        public IEnumerable<ComponentGeneratorOutput> CreateCsOutput(string body)
        {
            IEnumerable<AutoElementData> children = AutoElementFinder.GetChildren(body);
            IEnumerable<ComponentGeneratorOutput> totalChildren = new List<ComponentGeneratorOutput>();
            foreach (var child in children)
            {
                totalChildren = totalChildren.Union(CreateCsOutput(child.Selector, child.InnerChildrens) , new ComponentOutputComparer());
            }
            return totalChildren;
        }

        public IEnumerable<ComponentGeneratorOutput> CreateCsOutput(string selector, IEnumerable<AutoElementData> children)
        {
            var keyWord = GetKeyWordFromSelector(selector);
            if (children.Count() == 0) //not a new cs file
                return new List<ComponentGeneratorOutput>();

            IEnumerable<ElementSelectorData> childrenData = children.Select(ConvertToElementSelectorData);
            IEnumerable<ComponentGeneratorOutput> outputs = new List<ComponentGeneratorOutput>();
            foreach (var child in children)
            {
                outputs = outputs.Union(CreateCsOutput(child.Selector, child.InnerChildrens.ToList()), new ComponentOutputComparer());
            }
            ComponentGeneratorOutput parentOutput;
            if (fileCreators.ContainsKey(keyWord))
            {
                parentOutput = fileCreators[keyWord](selector, childrenData.ToArray());
            }
            else
            {                
                parentOutput = _defaultFileCreator(selector, childrenData.ToArray());
            }
            outputs = outputs.Union(new[] { parentOutput }, new ComponentOutputComparer());
            return outputs;
        }

        private string GetClassNameFromSelector(string selector)
        {
            string[] parts = selector.Split('-');
            return parts[2];
        }
        private string GetKeyWordFromSelector(string selector)
        {
            string[] parts = selector.Split('-');
            return parts[1];
        }
        private ElementSelectorData ConvertToElementSelectorData(AutoElementData data)
        {
            return new ElementSelectorData()
            {
                FullSelector = data.Selector,
                Name = GetClassNameFromSelector(data.Selector),
                Type = GetKeyWordFromSelector(data.Selector)
            };
        }
    }
}
