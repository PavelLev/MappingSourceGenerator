namespace MappingSourceGenerator;

public class MappingMethod
{
    public MappingMethod(
        string name,
        string modifiers,
        string parameterTypeFullName,
        string parameterName,
        string returnTypeFullName,
        MappingMethodKind kind,
        IReadOnlyCollection<MappingProperty>? properties,
        IReadOnlyCollection<string>? enumValues)
    {
        Name = name;
        Modifiers = modifiers;
        ParameterTypeFullName = parameterTypeFullName;
        ParameterName = parameterName;
        ReturnTypeFullName = returnTypeFullName;
        Kind = kind;
        Properties = properties;
        EnumValues = enumValues;
    }

    public string Name { get; }

    public string Modifiers { get; }

    public string ParameterTypeFullName { get; }

    public string ParameterName { get; }

    public string ReturnTypeFullName { get; }

    public MappingMethodKind Kind { get; }

    public IReadOnlyCollection<MappingProperty>? Properties { get; }

    public IReadOnlyCollection<string>? EnumValues { get; }
}