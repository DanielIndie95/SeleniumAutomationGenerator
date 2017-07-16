using Core.Models;

namespace Core
{
    public interface IComponentClassAppender
    {
        void AppendToClass(IComponentFileCreator parentClass, string selector, ElementSelectorData[] elements);
    }
}
