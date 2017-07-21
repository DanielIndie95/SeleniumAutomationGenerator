using BaseComponentsAddins;
using Core;
using Core.Utils;
using SeleniumAutomationGenerator.Generator.ClassAppenders;
using SeleniumAutomationGenerator.Generator.ComponentsGenerators;
using SeleniumAutomationGenerator.Generator.CustomClassAttributes;
using SeleniumAutomationGenerator.Generator.PropertyGenerators;

namespace SeleniumAutomationGenerator.Utils
{
    public static class BuiltInComponentsInserter
    {
        public static void InsertBuiltInComponents(IClassBuilder classBuilder)
        {
            ComponentsContainer.Instance.AddAddin(new InputAddin());
            ComponentsContainer.Instance.AddAddin(new LabelAddin());
            ComponentsContainer.Instance.AddAddin(new ButtonAddin());
            ComponentsContainer.Instance.AddAddin(new ListItemAddin());
            ComponentsContainer.Instance.AddAddin(new SelectItemAdding());
            ComponentsContainer.Instance.AddCustomAttribute(new VisibleElementAttribute());
            ComponentsContainer.Instance.AddCustomAttribute(new WaitUntilDisplayedElementAttribute());
            ComponentsFactory.Instance.AddComponentClassGeneratorKey("page",
                new PageGenerator(classBuilder, new DriverFindElementPropertyGenerator(Consts.DRIVER_FIELD_NAME),
                    Consts.PAGES_NAMESPACE));
            ComponentsFactory.Instance.AddComponentClassGeneratorKey("model",
                new ModelGenerator(classBuilder,
                    new ParentElementFindElementPropertyGenerator(Consts.DRIVER_FIELD_NAME,
                        Consts.PARENT_ELEMENT_FIELD_NAME), Consts.COMPONENTS_NAMESPACE, Consts.PARENT_ELEMENT_FIELD_NAME));
            ComponentsFactory.Instance.AddComponentClassGeneratorKey("comp",
            new ComponentGenerator(classBuilder,
                new ParentElementFindElementPropertyGenerator(Consts.DRIVER_FIELD_NAME,
                Consts.PARENT_ELEMENT_FIELD_NAME), Consts.COMPONENTS_NAMESPACE, Consts.PARENT_ELEMENT_FIELD_NAME), true);
            ComponentsFactory.Instance.AddComponentTypeAppenders(new ListClassAppender());
        }
    }
}