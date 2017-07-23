# SeleniumAutomationGenerator
Generate Infrastructure project from html

- Implement new IComponentAddin to create "in-class" properties and helpers methods
- Implement new IComponentFileCreator to create new classes
- Implement new IComponentClassAppender to create exceptions appenders for the IComponentsFileCreators - will ignore this types when generating the classes properties , so you can create your own in the Class appender implementation.
- Implement new IElementAttribute to create more helpers to the addin component

Add implementations to the ComponentsContainer instance
```csharp
var container = ComponentsContainer.Instance;
container.AddAddin(new InputAddin());
```

```csharp
container.AddCustomAttribute(new VisibleElementAttribute(container));
container.AddCustomAttribute(new WaitUntilDisplayedElementAttribute());
```
