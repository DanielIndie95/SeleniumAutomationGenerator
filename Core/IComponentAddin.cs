namespace Core
{
    public interface IComponentAddin
    {
        string AddinKey { get; }

        string Type { get; }

        string[] RequiredUsings { get; }

        string[] GenerateHelpers(string className, string selector , IPropertyGenerator generator);

        bool IsPropertyModifierPublic { get; }
        bool IsArrayedAddin { get; }
        bool CtorContainsDriver { get; }
    }
}
