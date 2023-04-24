﻿using System.Text;

namespace MappingSourceGenerator;

public class Emitter
{
    private const int DefaultStringBuilderCapacity = 1024;
    private const string OneLevelIndentation = "    ";

    private static readonly IReadOnlyCollection<string> RequiredUsingStatements = new[]
    {
        "using System;",
        "using System.Linq;",
    };

    private readonly StringBuilder _builder = new (DefaultStringBuilderCapacity);

    public (string BaseFileName, string Content) Emit(
        MappingClass mappingClass)
    {
        _builder.Clear();
        _builder.AppendLine("// <auto-generated/>");
        _builder.AppendLine("#pragma warning disable");
        _builder.AppendLine("#nullable enable");

        var nestedIndentation = string.Empty;

        if (!string.IsNullOrEmpty(mappingClass.Usings))
        {
            _builder.Append(mappingClass.Usings);

            foreach (var requiredUsingStatement in RequiredUsingStatements)
            {
                if (!mappingClass.Usings.Contains(requiredUsingStatement))
                {
                    _builder.AppendLine(requiredUsingStatement);
                }
            }
        }

        if (!string.IsNullOrWhiteSpace(mappingClass.Namespace))
        {
            _builder.Append($@"
namespace {mappingClass.Namespace};
");
        }

        _builder.Append($@"
partial {mappingClass.Keyword} {mappingClass.Name} {mappingClass.Constraints}
{{");

        var lastMappingMethod = mappingClass.Methods.Last();
        foreach (var mappingMethod in mappingClass.Methods)
        {
            GenerateMethod(mappingMethod, nestedIndentation);

            if (mappingMethod != lastMappingMethod)
            {
                _builder.Append(Environment.NewLine);
            }
        }

        _builder.Append(@"
}");

        var baseFullName = $"{mappingClass.Namespace}.{mappingClass.Name}";
        return (baseFullName, _builder.ToString());
    }

    private void GenerateMethod(MappingMethod mappingMethod, string nestedIndentation)
    {
        nestedIndentation += OneLevelIndentation;

        _builder.Append($@"
{nestedIndentation}{mappingMethod.Modifiers} {mappingMethod.ReturnTypeFullName} {mappingMethod.Name}(
{nestedIndentation}{OneLevelIndentation}this {mappingMethod.ParameterTypeFullName} {mappingMethod.ParameterName})");

        switch (mappingMethod.Kind)
        {
            case MappingMethodKind.Object:
                {
                    _builder.Append($@"
{nestedIndentation}{OneLevelIndentation}=> new(");

                    var lastMappingProperty = mappingMethod.Properties!.Last();
                    foreach (var mappingProperty in mappingMethod.Properties!)
                    {
                        var isLast = mappingProperty == lastMappingProperty;
                        GenerateConstructorArgument(mappingMethod, nestedIndentation, mappingProperty, isLast);
                    }

                    _builder.Append(");");
                    break;
                }

            case MappingMethodKind.Enum:
                {
                    _builder.Append($@"
{nestedIndentation}{OneLevelIndentation}=> {mappingMethod.ParameterName} switch
{nestedIndentation}{OneLevelIndentation}{{");

                    foreach (var enumValue in mappingMethod.EnumValues!)
                    {
                        _builder.Append($@"
{nestedIndentation}{OneLevelIndentation}{OneLevelIndentation}{mappingMethod.ParameterTypeFullName}.{enumValue} => {mappingMethod.ReturnTypeFullName}.{enumValue},");
                    }

                    _builder.Append($@"
{nestedIndentation}{OneLevelIndentation}{OneLevelIndentation}_ => throw new InvalidOperationException($""Unable to map {mappingMethod.ParameterTypeFullName}.{{{mappingMethod.ParameterName}}}""),
{nestedIndentation}{OneLevelIndentation}}};");

                    break;
                }

            default:
                {
                    throw new InvalidOperationException($"Unable to emit mapping method of kind {mappingMethod.Kind}");
                }
        }
    }

    private void GenerateConstructorArgument(
        MappingMethod mappingMethod,
        string nestedIndentation,
        MappingProperty mappingProperty,
        bool isLast)
    {
        _builder.Append($@"
{nestedIndentation}{OneLevelIndentation}{OneLevelIndentation}{mappingMethod.ParameterName}.{mappingProperty.PropertyName}");

        if (mappingProperty.ShouldUseNullForgivingOperator)
        {
            _builder.Append('!');
        }

        switch (mappingProperty.Kind)
        {
            case MappingPropertyKind.Direct:
                {
                    break;
                }

            case MappingPropertyKind.SingleItemMapping:
                {
                    _builder.Append($".{mappingProperty.MappingMethodName}()");
                    break;
                }

            case MappingPropertyKind.EnumerableMapping:
                {
                    _builder.Append($".Select({mappingProperty.MappingMethodName}).ToList()");
                    break;
                }

            default:
                {
                    throw new InvalidOperationException($"Unable to emit constructor argument for mapping property of kind {mappingMethod.Kind}");
                }
        }

        if (!isLast)
        {
            _builder.Append(',');
        }
    }
}