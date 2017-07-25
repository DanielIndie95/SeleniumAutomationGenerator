using System.Collections.Generic;
using System.Linq;
using Core;
using Core.Models;
using SeleniumAutomationGenerator.Utils;
using MoreLinq;
using SeleniumAutomationGenerator.ComponentFactorySteps;

namespace SeleniumAutomationGenerator
{
    public class ComponentsFactory : IComponentsFactory, IComponentFactoryCreatorStep
    {
        private readonly List<IComponentFactoryCreatorStep> _creatorsSteps;
        private readonly List<IComponentFactoryAppenderStep> _appendersSteps;

        public ComponentsFactory()
        {
            _creatorsSteps = new List<IComponentFactoryCreatorStep>();
            _appendersSteps = new List<IComponentFactoryAppenderStep>();
        }

        public ComponentsFactory AddStep(IComponentFactoryAppenderStep step)
        {
            _appendersSteps.Add(step);
            return this;
        }
        public ComponentsFactory AddStep(IComponentFactoryCreatorStep step)
        {
            _creatorsSteps.Add(step);
            return this;
        }

        public IEnumerable<ComponentGeneratorOutput> CreateCsOutput(string body)
        {
            IEnumerable<AutoElementData> children = AutoElementFinder.GetChildren(body);
            return GenerateClassesForElements(children);
        }

        public IEnumerable<ComponentGeneratorOutput> InvokeStep(AutoElementData current,
            IComponentFileCreator parentClassCreator, IComponentFactoryCreatorStep next = null)
        {
            _appendersSteps
                .Where(step => step.ShouldInvokeStep(current, parentClassCreator))
                .ForEach(step => step.InvokeStep(current, parentClassCreator));

            var result = _creatorsSteps.FirstOrDefault(step => step.ShouldInvokeStep(current, parentClassCreator))
                ?.InvokeStep(current, parentClassCreator, this);

            return result ?? Enumerable.Empty<ComponentGeneratorOutput>();
        }

        public bool ShouldInvokeStep(AutoElementData rootElement, IComponentFileCreator parent)
        {
            return true;
        }

        private IEnumerable<ComponentGeneratorOutput> GenerateClassesForElements(IEnumerable<AutoElementData> children)
        {
            HashSet<ComponentGeneratorOutput> outputs = new HashSet<ComponentGeneratorOutput>(new ComponentOutputComparer());
            foreach (AutoElementData child in children)
            {
                outputs.UnionWith(InvokeStep(child, null));
            }
            return outputs;
        }
    }
}