using System.Collections.Generic;
using Core;
using Core.Models;

namespace SeleniumAutomationGenerator
{
    public interface IComponentsFactory
    {
        void AddComponentClassGeneratorKey(string key, IComponentFileCreator newComponentFileCreator);
        void AddComponentTypeAppenders(string type, IComponentClassAppender classAppender);
        IEnumerable<ComponentGeneratorOutput> CreateCsOutput(string body);
    }
}