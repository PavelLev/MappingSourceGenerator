namespace MappingSourceGenerator;

public record MappingMethod(
    string Name,
    string Modifiers,
    string ParameterTypeFullName,
    string ParameterName,
    string ReturnTypeFullName,
    MappingMethodKind Kind,
    IReadOnlyCollection<MappingProperty>? Properties,
    IReadOnlyCollection<string>? EnumValues);