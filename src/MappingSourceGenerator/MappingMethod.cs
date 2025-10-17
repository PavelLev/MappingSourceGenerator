using Microsoft.CodeAnalysis;

namespace MappingSourceGenerator;

public record MappingMethod(
    string ClassName,
    string[] ClassContainingNames,
    string Name,
    Accessibility Accessibility,
    bool IsPartial,
    MappingModel ParameterTypeModel,
    string ParameterName,
    MappingModel ReturnTypeModel,
    MappingMethodKind Kind,
    MappingProperty[]? Properties,
    string[]? EnumValues)
{
    // Forcing default Equality comparison.
    // Record by default doesn't handle collections properly
    // so there is no point in having slow and incorrect implementation by default.
    // Proper implementation is done by MappingMethodEqualityComparer.
    public virtual bool Equals(MappingMethod? other)
    {
        return base.Equals(other);
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }
}