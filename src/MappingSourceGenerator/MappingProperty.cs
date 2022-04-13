namespace MappingSourceGenerator;

public class MappingProperty
{
    public MappingProperty(
        string propertyName,
        MappingPropertyKind kind,
        string? mappingMethodName = default,
        bool shouldUseNullForgivingOperator = default)
    {
        PropertyName = propertyName;
        Kind = kind;
        MappingMethodName = mappingMethodName;
        ShouldUseNullForgivingOperator = shouldUseNullForgivingOperator;
    }

    public string PropertyName { get; }

    public MappingPropertyKind Kind { get; }

    public string? MappingMethodName { get; }

    public bool ShouldUseNullForgivingOperator { get; }
}