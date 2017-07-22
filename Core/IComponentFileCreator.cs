using Core.Models;

namespace Core
{
    public interface IComponentFileCreator
    {
        IPropertyGenerator PropertyGenerator { get; }
        ComponentGeneratorOutput GenerateComponentClass(string selector, ElementSelectorData[] elements);
        void AddProperty(string property);
        void AddMethod(string method);
        void InsertToCtor(string bulk);
        void AddUsing(string usingName);
        IComponentAddin MakeAddin(string selector);
    }
}
