using Core.Models;

namespace Core
{
    public interface IComponentClassAppender
    {
        string Identifier { get; }
        void AppendToClass(IComponentFileCreator parentClass, AutoElementData appenderElement);
    }
}
