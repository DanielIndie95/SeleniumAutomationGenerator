using Core.Models;
using SeleniumAutomationGenerator.Models;

namespace Core
{
    public interface IComponentClassAppender
    {
        void AppendToClass(IComponentFileCreator parentClass, AutoElementData appenderElement);
    }
}
