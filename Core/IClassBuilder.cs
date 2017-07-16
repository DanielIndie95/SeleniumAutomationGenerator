namespace Core
{
    public interface IClassBuilder
    {
        IClassBuilder AddUsings(params string[] usings);
        IClassBuilder AddMethods(params string[] methods);
        IClassBuilder AddFields(params string[] fields);
        IClassBuilder AddProperties(params string[] properties);
        IClassBuilder AddCtor(string ctor);
        IClassBuilder SetNamesapce(string namespaceName);
        IClassBuilder SetClassName(string className);

        string Build();
    }
}
