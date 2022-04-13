namespace MappingSourceGenerator;

public class MappingClass
{
    public MappingClass(
        string name,
        string keyword,
        string constraints,
        string @namespace,
        string usings,
        IReadOnlyCollection<MappingMethod> methods)
    {
        Name = name;
        Keyword = keyword;
        Constraints = constraints;
        Namespace = @namespace;
        Usings = usings;
        Methods = methods;
    }

    public string Name { get; }

    public string Keyword { get; }

    public string Constraints { get; }

    public string Namespace { get; }

    public string Usings { get; }

    public IReadOnlyCollection<MappingMethod> Methods { get; }
}