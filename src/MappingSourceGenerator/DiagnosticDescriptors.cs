using Microsoft.CodeAnalysis;

namespace MappingSourceGenerator;

public static class DiagnosticDescriptors
{
    public static DiagnosticDescriptor MappingMethodShouldBePartial { get; } = new(
        "MAPGEN1000",
        "Invalid mapping method",
        "Mapping method should be partial",
        nameof(MappingGenerator),
        DiagnosticSeverity.Error,
        true);

    public static DiagnosticDescriptor MappingMethodShouldNotReturnVoid { get; } = new(
        "MAPGEN1001",
        "Invalid mapping method",
        "Mapping method shouldn't return void",
        nameof(MappingGenerator),
        DiagnosticSeverity.Error,
        true);

    public static DiagnosticDescriptor MappingMethodShouldBeExtension { get; } = new(
        "MAPGEN1002",
        "Invalid mapping method",
        "Mapping method should be an extension method",
        nameof(MappingGenerator),
        DiagnosticSeverity.Error,
        true);

    public static DiagnosticDescriptor MappingMethodContainingClassShouldNotBeNested { get; } = new(
        "MAPGEN1003",
        "Invalid mapping method containing class",
        "Mapping method containing class shouldn't be nested",
        nameof(MappingGenerator),
        DiagnosticSeverity.Error,
        true);
    
    public static DiagnosticDescriptor MappingMethodShouldHaveSingleParameter { get; } = new(
        "MAPGEN1010",
        "Invalid mapping method parameters",
        "Mapping method should contain single parameter",
        nameof(MappingGenerator),
        DiagnosticSeverity.Error,
        true);

    public static DiagnosticDescriptor MappingMethodShouldNotBeGeneric { get; } = new(
        "MAPGEN1011",
        "Invalid mapping method parameters",
        "Mapping method shouldn't contain generic type parameters",
        nameof(MappingGenerator),
        DiagnosticSeverity.Error,
        true);

    public static DiagnosticDescriptor MappingMethodParameterShouldNotBeNullable { get; } = new(
        "MAPGEN1012",
        "Invalid mapping method parameters",
        "Mapping method parameter shouldn't be nullable",
        nameof(MappingGenerator),
        DiagnosticSeverity.Error,
        true);

    public static DiagnosticDescriptor MappingMethodParameterTypeNotSupported { get; } = new(
        "MAPGEN1013",
        "Invalid mapping method parameters",
        "Parameter type {0} of mapping method should be enum, class or struct but found {1}",
        nameof(MappingGenerator),
        DiagnosticSeverity.Error,
        true);

    public static DiagnosticDescriptor MappingMethodReturnTypeContainsMultipleSuitableConstructors { get; } = new(
        "MAPGEN1020",
        "Invalid mapping method return type",
        "Return type {0} of mapping method contains multiple public non-default constructors",
        nameof(MappingGenerator),
        DiagnosticSeverity.Error,
        true);

    public static DiagnosticDescriptor MappingMethodReturnTypeSuitableConstructorNotFound { get; } = new(
        "MAPGEN1021",
        "Invalid mapping method return type",
        "Return type {0} of mapping method should have public non-default constructor",
        nameof(MappingGenerator),
        DiagnosticSeverity.Error,
        true);

    public static DiagnosticDescriptor MappingMethodReturnTypeNotSupported { get; } = new(
        "MAPGEN1022",
        "Invalid mapping method return type",
        "Return type {0} of mapping method should be enum, class or struct",
        nameof(MappingGenerator),
        DiagnosticSeverity.Error,
        true);

    public static DiagnosticDescriptor ConstructorMappingNotFound { get; } = new(
        "MAPGEN1030",
        "Invalid mapping",
        "Mapping for '{0}' parameter of '{1}' constructor is not found in type '{2}'",
        nameof(MappingGenerator),
        DiagnosticSeverity.Error,
        true);

    public static DiagnosticDescriptor EnumMappingNotFound { get; } = new(
        "MAPGEN1031",
        "Invalid mapping",
        "Mapping for '{0}' value of '{1}' enum is not found in enum '{2}'",
        nameof(MappingGenerator),
        DiagnosticSeverity.Error,
        true);

    public static DiagnosticDescriptor NullableToNonNullableMapping { get; } = new(
        "MAPGEN1032",
        "Invalid mapping",
        "Trying to map nullable property '{0}' of type '{1}' into corresponding non-nullable constructor parameter of type '{2}' at path '{3}'",
        nameof(MappingGenerator),
        DiagnosticSeverity.Error,
        true);

    public static DiagnosticDescriptor EnumerableWithNullableItemMappingNotSupported { get; } = new(
        "MAPGEN1033",
        "Invalid mapping",
        "Trying to map enumerable with nullable items property '{0}' of type '{1}' at path '{2}' which is not supported",
        nameof(MappingGenerator),
        DiagnosticSeverity.Error,
        true);

    public static DiagnosticDescriptor EnumerableToNonEnumerableMappingNotSupported { get; } = new(
        "MAPGEN1034",
        "Invalid mapping",
        "Trying to map enumerable property '{0}' of type '{1}' into non-enumerable type '{2}' at path '{3}' which is not supported",
        nameof(MappingGenerator),
        DiagnosticSeverity.Error,
        true);

    public static DiagnosticDescriptor NonEnumerableToEnumerableMappingNotSupported { get; } = new(
        "MAPGEN1035",
        "Invalid mapping",
        "Trying to map non-enumerable property '{0}' of type '{1}' into enumerable type '{2}' at path '{3}' which is not supported",
        nameof(MappingGenerator),
        DiagnosticSeverity.Error,
        true);
}