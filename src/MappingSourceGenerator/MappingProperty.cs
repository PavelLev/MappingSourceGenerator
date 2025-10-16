namespace MappingSourceGenerator;

public record MappingProperty(
    string PropertyName,
    MappingPropertyKind Kind,
    string? MappingMethodName,
    bool ShouldUseNullConditionalOperator,
    bool ShouldUseListForCollectionMapping);