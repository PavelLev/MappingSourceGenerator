namespace MappingSourceGenerator;

public record MappingProperty(
    string PropertyName,
    MappingPropertyKind Kind,
    string? MappingMethodName = default,
    bool ShouldUseNullConditionalOperator = default);