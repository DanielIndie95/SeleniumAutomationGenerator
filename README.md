# SeleniumAutomationGenerator
Generate Infrastructure project from html

Implement new IComponentAddin to create "in-class" properties and helpers methods
Implement new IComponentFileCreator to create new classes
Implement new IComponentClassAppender to create exceptions appenders for the IComponentsFileCreators - will ignore this types when generating the classes properties , so you can create your own in the Class appender implementation.

Currently, you can add the implementations programmatically only
- Add addins to the ComponentAddinsContainer singleton
- Add File Creators and Class Appenders in the ComponentFactory singleton
