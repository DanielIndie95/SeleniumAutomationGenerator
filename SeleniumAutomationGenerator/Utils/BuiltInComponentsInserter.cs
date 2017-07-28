using BaseComponentsAddins;
using Core;
using Core.Utils;
using SeleniumAutomationGenerator.Builders;
using SeleniumAutomationGenerator.ComponentFactorySteps;
using SeleniumAutomationGenerator.Generator.ClassAppenders;
using SeleniumAutomationGenerator.Generator.ComponentsGenerators;
using SeleniumAutomationGenerator.Generator.CustomClassAttributes;
using SeleniumAutomationGenerator.Generator.PropertyGenerators;

namespace SeleniumAutomationGenerator.Utils
{
    public static class BuiltInComponentsInserter
    {
        public static void InsertBuiltInComponents(ComponentsFactory factory)
        {

            var container = ComponentsContainer.Instance;
            container.AddAddin(new InputAddin());
            container.AddAddin(new LabelAddin());
            container.AddAddin(new ButtonAddin());
            container.AddAddin(new ListItemAddin());
            container.AddAddin(new SelectItemAdding());
            container.AddCustomAttribute(new VisibleElementAttribute(container));
            container.AddCustomAttribute(new WaitUntilDisplayedElementAttribute());
            container.AddFileCreatorComponent("page",
                new PageGenerator(new BasicClassBuilder(), new DriverFindElementPropertyGenerator(Consts.DRIVER_FIELD_NAME),
                    container,
                    Consts.PAGES_NAMESPACE));
            container.AddFileCreatorComponent("model",
                new ModelGenerator(new BasicClassBuilder(),
                    new ParentElementFindElementPropertyGenerator(Consts.DRIVER_FIELD_NAME,
                        Consts.PARENT_ELEMENT_FIELD_NAME),
                    container,
                    Consts.COMPONENTS_NAMESPACE, Consts.PARENT_ELEMENT_FIELD_NAME));
            container.AddFileCreatorComponent("comp",
            new ComponentGenerator(new BasicClassBuilder(),
                new ParentElementFindElementPropertyGenerator(Consts.DRIVER_FIELD_NAME,
                Consts.PARENT_ELEMENT_FIELD_NAME),
                container,
                Consts.COMPONENTS_NAMESPACE, Consts.PARENT_ELEMENT_FIELD_NAME), true);
            container.AddComponentTypeAppenders(new ListClassAppender(container));

            factory.AddStep(new CustomElementAttributeAppenderStep(container))
                .AddStep(new BasicElementCreatorStep())
                .AddStep(new AppenderCreatorStep(container))
                .AddStep(new FileCreatorFactoryStep(container, container, container));
        }
    }
}