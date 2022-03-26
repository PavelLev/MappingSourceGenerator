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

    public static DiagnosticDescriptor MappingMethodShouldntReturnVoid { get; } = new(
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

    public static DiagnosticDescriptor MappingMethodShouldHaveSingleParameter { get; } = new(
        "MAPGEN1010",
        "Invalid mapping method parameters",
        "Mapping method should contain single parameter",
        nameof(MappingGenerator),
        DiagnosticSeverity.Error,
        true);

    public static DiagnosticDescriptor MappingMethodShouldntBeGeneric { get; } = new(
        "MAPGEN1011",
        "Invalid mapping method parameters",
        "Mapping method shouldn't contain generic type parameters",
        nameof(MappingGenerator),
        DiagnosticSeverity.Error,
        true);

    public static DiagnosticDescriptor MappingMethodParameterShouldntBeNullable { get; } = new(
        "MAPGEN1012",
        "Invalid mapping method parameters",
        "Mapping method parameter shouldn't be nullable",
        nameof(MappingGenerator),
        DiagnosticSeverity.Error,
        true);

    public static DiagnosticDescriptor MappingMethodParameterTypeNotSupported { get; } = new(
        "MAPGEN1022",
        "Invalid mapping method parameters",
        "Parameter type {0} of mapping method should be enum, class or struct",
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
        "Mapping for '{0}' parameter of '{1}' constructor is not found in '{2}",
        nameof(MappingGenerator),
        DiagnosticSeverity.Error,
        true);

    public static DiagnosticDescriptor EnumMappingNotFound { get; } = new(
        "MAPGEN1031",
        "Invalid mapping",
        "Mapping for '{0}' value of '{1}' enum is not found in '{2}",
        nameof(MappingGenerator),
        DiagnosticSeverity.Error,
        true);
}