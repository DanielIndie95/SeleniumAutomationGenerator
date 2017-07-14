using SeleniumAutomationGenerator.Generator;
using SeleniumAutomationGenerator.Models;
using System;
using System.Collections.Generic;

namespace SeleniumAutomationGenerator
{
    public class ComponentsFactory
    {
        Dictionary<string, Func<string, ElementSelectorData[], ComponentGeneratorOutput>> fileCreators;
        Func<string, ElementSelectorData[], ComponentGeneratorOutput> _defaultFileCreator;
        public ComponentsFactory(IAddinsContainer container, IPropertyGenerator propertyGenerator)
        {
            fileCreators = new Dictionary<string, Func<string, ElementSelectorData[], ComponentGeneratorOutput>>();
            AddComponentGeneratorKey("page", (selector, elements) =>
                                            new BasicPageGenerator(container, propertyGenerator, Consts.PAGES_NAMESPACE)
                                               .GenerateComponentClass(GetClassNameFromSelector(selector), elements));

            AddComponentGeneratorKey("model", (selector, elements) =>
                                            new BasicModelGenerator(container, propertyGenerator, Consts.PAGES_NAMESPACE, Consts.PARENT_ELEMENT_FIELD_NAME)
                                               .GenerateComponentClass(GetClassNameFromSelector(selector), elements));            

            _defaultFileCreator = (selector, elements) =>
                                           new BasicComponentGenerator(container, propertyGenerator, Consts.PAGES_NAMESPACE, Consts.PARENT_ELEMENT_FIELD_NAME)
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
        public ComponentGeneratorOutput CreateCsOutput(string selector , string innerBody)
        {
            throw new NotImplementedException();
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
    }
}
