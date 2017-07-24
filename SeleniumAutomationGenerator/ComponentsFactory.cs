﻿using System.Collections.Generic;
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
            return result ?? new List<ComponentGeneratorOutput>();
        }

        public bool ShouldInvokeStep(AutoElementData rootElement, IComponentFileCreator parent)
        {
            return true;
        }

        private IEnumerable<ComponentGeneratorOutput> GenerateClassesForElements(IEnumerable<AutoElementData> children, IComponentFileCreator parent = null)
        {
            IEnumerable<ComponentGeneratorOutput> outputs = new List<ComponentGeneratorOutput>();
            foreach (AutoElementData child in children)
            {
                outputs = outputs.Union(InvokeStep(child, parent), new ComponentOutputComparer());
            }
            return outputs;
        }
    }
}