using System.Collections.Generic;
using Core;
using Core.Models;

namespace SeleniumAutomationGenerator
{
    public interface IComponentsFactory
    {
        void AddComponentClassGeneratorKey(string key, IComponentFileCreator newComponentFileCreator, bool setAsDefault = false);
        void AddComponentTypeAppenders(IComponentClassAppender classAppender);
        IEnumerable<ComponentGeneratorOutput> CreateCsOutput(string body);
    }
}