﻿using System.Text;
using Microsoft.CodeAnalysis;

namespace MappingSourceGenerator;

public class ManualInterpolationEmitter : IEmitter
{
    private const int DefaultContentStringBuilderCapacity = 1024;
    private const int DefaultHintNameStringBuilderCapacity = 100;

    private static readonly IReadOnlyCollection<string> RequiredUsingStatements = new[]
    {
        "using System;",
        "using System.Linq;",
    };

    private readonly StringBuilder _contentStringBuilder = new (DefaultContentStringBuilderCapacity);
    private readonly StringBuilder _hintNameStringBuilder = new (DefaultHintNameStringBuilderCapacity);

    public void Emit(
        Action<string, string> addSource,
        IReadOnlyCollection<MappingMethod> mappingMethods,
        CancellationToken cancellationToken)
    {
        var firstMappingMethod = mappingMethods.First();
        var (currentClassName, currentClassContainingNames) =
            (firstMappingMethod.ClassName, firstMappingMethod.ClassContainingNames);
        AppendClassHeaders(currentClassName, currentClassContainingNames);
        
        foreach (var mappingMethod in mappingMethods)
        {
            cancellationToken.ThrowIfCancellationRequested();
            
            if (mappingMethod.ClassName != currentClassName
                || !mappingMethod.ClassContainingNames.SequenceEqual(currentClassContainingNames))
            {
                CompleteClassAndAddSource(addSource, currentClassName, currentClassContainingNames);

                (currentClassName, currentClassContainingNames) =
                    (mappingMethod.ClassName, mappingMethod.ClassContainingNames);
                AppendClassHeaders(currentClassName, currentClassContainingNames);
            }
            else if (mappingMethod != firstMappingMethod)
            {
                _contentStringBuilder.AppendLine();
            }
            
            AppendMethod(mappingMethod);
        }
        
        CompleteClassAndAddSource(addSource, currentClassName, currentClassContainingNames);

        void CompleteClassAndAddSource(
            Action<string, string> addSource,
            string className,
            IReadOnlyList<string> classContainingNames)
        {
            _contentStringBuilder.Append('}');

            var hintName = GetHintName(className, classContainingNames);
            var generatedSource = _contentStringBuilder.ToString();
            addSource(hintName, generatedSource);
            _contentStringBuilder.Clear();
        }
    }

    private void AppendClassHeaders(string className, IReadOnlyList<string> classContainingNames)
    {
        _contentStringBuilder.AppendLine("// <auto-generated/>")
            .AppendLine("#pragma warning disable")
            .AppendLine("#nullable enable");

        foreach (var requiredUsingStatement in RequiredUsingStatements)
        {
            _contentStringBuilder.AppendLine(requiredUsingStatement);
        }
        
        // @$"
        // namespace {mappingClassNamespace};
        //
        // "
        _contentStringBuilder.AppendLine()
            .Append("namespace ")
            .AppendCollectionWithSeparator(classContainingNames, '.')
            .AppendLine(";")
            .AppendLine();

        // $@"// partial class {mappingClassName}
        // {{
        // "
        _contentStringBuilder
            .Append("partial class ")
            .AppendLine(className)
            .AppendLine("{");
    }

    private void AppendMethod(MappingMethod mappingMethod, int indentationLevel = 1)
    {
        // $@"{nestedIndentation}{mappingMethod.Modifiers} {mappingMethod.ReturnTypeNamespace}.{mappingMethod.ReturnTypeName} {mappingMethod.Name}(
        // {nestedIndentation}{EmitterConstants.OneLevelIndentation}this {mappingMethod.ParameterTypeNamespace}.{mappingMethod.ParameterTypeName} {mappingMethod.ParameterName})
        // "
        // where nestedIndentation is EmitterConstants.OneLevelIndentation repeated indentationLevel times
        _contentStringBuilder
            .AppendRepeated(EmitterConstants.OneLevelIndentation, indentationLevel)
            .Append(GetAccessibilityString(mappingMethod.Accessibility))
            .Append(" static ");

        if (mappingMethod.IsPartial)
        {
            _contentStringBuilder.Append("partial ");
        }
            
        _contentStringBuilder
            .AppendCollectionWithSeparator(mappingMethod.ReturnTypeContainingNames, '.')
            .Append('.')
            .Append(mappingMethod.ReturnTypeName)
            .Append(' ')
            .Append(mappingMethod.Name)
            .AppendLine("(")
            .AppendRepeated(EmitterConstants.OneLevelIndentation, indentationLevel + 1)
            .Append("this ")
            .AppendCollectionWithSeparator(mappingMethod.ParameterTypeContainingNames, '.')
            .Append('.')
            .Append(mappingMethod.ParameterTypeName)
            .Append(' ')
            .Append(char.ToLower(mappingMethod.ParameterName[0]))
            .Append(mappingMethod.ParameterName, 1, mappingMethod.ParameterName.Length - 1)
            .AppendLine(")");

        switch (mappingMethod.Kind)
        {
            case MappingMethodKind.Object:
                {
                    _contentStringBuilder.AppendRepeated(EmitterConstants.OneLevelIndentation, indentationLevel + 1)
                        .AppendLine("=> new(");

                    var lastMappingProperty = mappingMethod.Properties!.Last();
                    foreach (var mappingProperty in mappingMethod.Properties!)
                    {
                        AppendConstructorArgument(mappingMethod, mappingProperty, indentationLevel);

                        var isLast = mappingProperty == lastMappingProperty;
                        if (!isLast)
                        {
                            _contentStringBuilder.AppendLine(",");
                        }
                    }

                    _contentStringBuilder.AppendLine(");");
                    break;
                }

            case MappingMethodKind.Enum:
                {
                    // $@"{nestedIndentation}=> {mappingMethod.ParameterName} switch
                    // {nestedIndentation}{{
                    // "
                    // where nestedIndentation is EmitterConstants.OneLevelIndentation repeated indentationLevel + 1 times
                    _contentStringBuilder.AppendRepeated(EmitterConstants.OneLevelIndentation, indentationLevel + 1)
                        .Append("=> ")
                        .Append(char.ToLower(mappingMethod.ParameterName[0]))
                        .Append(mappingMethod.ParameterName, 1, mappingMethod.ParameterName.Length - 1)
                        .AppendLine(" switch")
                        .AppendRepeated(EmitterConstants.OneLevelIndentation, indentationLevel + 1)
                        .AppendLine("{");

                    foreach (var enumValue in mappingMethod.EnumValues!)
                    {
                        // $@"{nestedIndentation}{mappingMethod.ParameterTypeNamespace}.{mappingMethod.ParameterTypeName}.{enumValue} => {mappingMethod.ReturnTypeNamespace}.{mappingMethod.ReturnTypeName}.{enumValue},
                        // "
                        // where nestedIndentation is EmitterConstants.OneLevelIndentation repeated indentationLevel + 2 times
                        _contentStringBuilder.AppendRepeated(EmitterConstants.OneLevelIndentation, indentationLevel + 2)
                            .AppendCollectionWithSeparator(mappingMethod.ParameterTypeContainingNames, '.')
                            .Append('.')
                            .Append(mappingMethod.ParameterTypeName)
                            .Append('.')
                            .Append(enumValue)
                            .Append(" => ")
                            .AppendCollectionWithSeparator(mappingMethod.ReturnTypeContainingNames, '.')
                            .Append('.')
                            .Append(mappingMethod.ReturnTypeName)
                            .Append('.')
                            .Append(enumValue)
                            .AppendLine(",");
                    }

                    // $@"{nestedIndentation}{OneLevelIndentation}_ => throw new InvalidOperationException($""Unable to map {mappingMethod.ParameterTypeNamespace}.{mappingMethod.ParameterTypeName}.{{{mappingMethod.ParameterName}}}""),
                    // {nestedIndentation}}};
                    // "
                    // where nestedIndentation is EmitterConstants.OneLevelIndentation repeated indentationLevel + 1 times
                    _contentStringBuilder.AppendRepeated(EmitterConstants.OneLevelIndentation, indentationLevel + 2)
                        .Append("_ => throw new InvalidOperationException($\"Unable to map ")
                        .AppendCollectionWithSeparator(mappingMethod.ParameterTypeContainingNames, '.')
                        .Append('.')
                        .Append(mappingMethod.ParameterTypeName)
                        .Append(".{")
                        .Append(char.ToLower(mappingMethod.ParameterName[0]))
                        .Append(mappingMethod.ParameterName, 1, mappingMethod.ParameterName.Length - 1)
                        .AppendLine("}\"),")
                        .AppendRepeated(EmitterConstants.OneLevelIndentation, indentationLevel + 1)
                        .AppendLine("};");
                    break;
                }

            default:
                {
                    throw new InvalidOperationException($"Unable to emit mapping method of kind {mappingMethod.Kind}");
                }
        }
    }

    private string GetAccessibilityString(Accessibility accessibility)
    {
        return accessibility switch
        {
            Accessibility.Private => "private",
            Accessibility.ProtectedAndInternal => "private protected",
            Accessibility.Protected => "protected",
            Accessibility.Internal => "internal",
            Accessibility.ProtectedOrInternal => "protected internal",
            Accessibility.Public => "public",
            _ => throw new InvalidOperationException($"Unable to find string for accessibility {accessibility}"),
        };
    }

    private void AppendConstructorArgument(
        MappingMethod mappingMethod,
        MappingProperty mappingProperty,
        int indentationLevel)
    {
        // $"{nestedIndentation}{mappingMethod.ParameterName}.{mappingProperty.PropertyName}"
        // where nestedIndentation is EmitterConstants.OneLevelIndentation repeated indentationLevel + 2 times
        _contentStringBuilder.AppendRepeated(
                EmitterConstants.OneLevelIndentation,
                indentationLevel + 2)
            .Append(char.ToLower(mappingMethod.ParameterName[0]))
            .Append(mappingMethod.ParameterName, 1, mappingMethod.ParameterName.Length - 1)
            .Append('.')
            .Append(mappingProperty.PropertyName);

        if (mappingProperty.ShouldUseNullConditionalOperator)
        {
            _contentStringBuilder.Append('?');
        }

        switch (mappingProperty.Kind)
        {
            case MappingPropertyKind.Direct:
                {
                    break;
                }

            case MappingPropertyKind.SingleItemMapping:
                {
                    // $".{mappingProperty.MappingMethodName}()"
                    _contentStringBuilder.Append('.')
                        .Append(mappingProperty.MappingMethodName)
                        .Append("()");
                    break;
                }

            case MappingPropertyKind.EnumerableMapping:
                {
                    _contentStringBuilder.Append(".Select(")
                        .Append(mappingProperty.MappingMethodName)
                        .Append(").ToArray()");
                    break;
                }

            default:
                {
                    throw new InvalidOperationException($"Unable to emit constructor argument for mapping property of kind {mappingMethod.Kind}");
                }
        }
    }

    private string GetHintName(string className, IReadOnlyList<string> classContainingNames)
    {
        _hintNameStringBuilder.AppendCollectionWithSeparator(classContainingNames, '.')
            .Append('.')
            .Append(className)
            .Append(".g.cs");

        var hintName = _hintNameStringBuilder.ToString();
        _hintNameStringBuilder.Clear();
        return hintName;
    }
}