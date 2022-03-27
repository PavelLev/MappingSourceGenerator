namespace MappingSourceGenerator;

public record MappingClass(
    string Name,
    string Keyword,
    string Constraints,
    string Namespace,
    string Usings,
    IReadOnlyCollection<MappingMethod> Methods);