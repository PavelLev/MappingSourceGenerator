using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MappingSourceGenerator;

public class Parser
{
    private const string GenerateMappingAttributeFullName = "MappingSourceGenerator.Markers.GenerateMappingAttribute";

    private readonly CancellationToken _cancellationToken;
    private readonly Compilation _compilation;
    private readonly Action<Diagnostic> _reportDiagnostic;
    private readonly INamedTypeSymbol _genericEnumerable;

    public Parser(Compilation compilation, Action<Diagnostic> reportDiagnostic, CancellationToken cancellationToken)
    {
        _compilation = compilation;
        _cancellationToken = cancellationToken;
        _reportDiagnostic = reportDiagnostic;
        _genericEnumerable = compilation.GetSpecialType(SpecialType.System_Collections_Generic_IEnumerable_T);
    }

    public static bool IsSyntaxTargetForGeneration(SyntaxNode node) =>
        node is MethodDeclarationSyntax { AttributeLists.Count: > 0 };

    public static ClassDeclarationSyntax? GetSemanticTargetForGeneration(GeneratorSyntaxContext context)
    {
        var methodDeclarationSyntax = (MethodDeclarationSyntax)context.Node;

        foreach (var attributeListSyntax in methodDeclarationSyntax.AttributeLists)
        {
            foreach (var attributeSyntax in attributeListSyntax.Attributes)
            {
                if (context.SemanticModel.GetSymbolInfo(attributeSyntax).Symbol is not IMethodSymbol attributeSymbol)
                {
                    continue;
                }

                var fullName = attributeSymbol.ContainingType.ToDisplayString();

                if (fullName == GenerateMappingAttributeFullName)
                {
                    return methodDeclarationSyntax.Parent as ClassDeclarationSyntax;
                }
            }
        }

        return null;
    }

    public IReadOnlyCollection<MappingClass> GetMapperClasses(IEnumerable<ClassDeclarationSyntax> classes)
    {
        var generateMappingAttribute = _compilation.GetBestTypeByMetadataName(GenerateMappingAttributeFullName);
        if (generateMappingAttribute == null)
        {
            // nothing to do if this type isn't available
            return Array.Empty<MappingClass>();
        }

        var mappingClasses = new List<MappingClass>();

        foreach (var group in classes.GroupBy(x => x.SyntaxTree))
        {
            var semanticModel = _compilation.GetSemanticModel(group.Key);

            foreach (var classDeclarationSyntax in group)
            {
                _cancellationToken.ThrowIfCancellationRequested();

                var methodByDeclarationSyntax = new Dictionary<MethodDeclarationSyntax, IMethodSymbol>();
                foreach (var memberDeclarationSyntax in classDeclarationSyntax.Members)
                {
                    var methodDeclarationSyntax = memberDeclarationSyntax as MethodDeclarationSyntax;
                    if (methodDeclarationSyntax is null)
                    {
                        continue;
                    }
                    var methodSymbol = semanticModel.GetDeclaredSymbol(methodDeclarationSyntax, _cancellationToken) as IMethodSymbol;

                    if (methodSymbol is null)
                    {
                        continue;
                    }

                    methodByDeclarationSyntax.Add(methodDeclarationSyntax, methodSymbol);
                }

                var allMappingMethods = new List<MappingMethod>();
                foreach (var (methodDeclarationSyntax, methodSymbol) in methodByDeclarationSyntax)
                {
                    if (!HasGenerateMappingAttribute(methodSymbol))
                    {
                        continue;
                    }

                    var hasErrors = IsMethodSignatureValid(methodSymbol, methodDeclarationSyntax);

                    if (hasErrors)
                    {
                        continue;
                    }

                    var parameter = methodSymbol.Parameters[0];
                    var (mappingMethods, tryGetMappingMethodErrors) = TryGetMappingMethod(
                        classDeclarationSyntax,
                        methodSymbol.Name,
                        parameter.Type,
                        parameter.Name,
                        parameter.Name,
                        methodSymbol.ReturnType,
                        methodDeclarationSyntax.Modifiers,
                        methodByDeclarationSyntax);

                    if (tryGetMappingMethodErrors is not null)
                    {
                        foreach (var (diagnosticDescriptor, argument0, argument1, argument2) in tryGetMappingMethodErrors)
                        {
                            ReportDiagnostic(
                                diagnosticDescriptor,
                                methodDeclarationSyntax.GetLocation(),
                                argument0,
                                argument1,
                                argument2);
                        }

                        continue;
                    }

                    foreach (var mappingMethod in mappingMethods!)
                    {
                        if (!allMappingMethods.Any(_ =>
                            _.Name == mappingMethod.Name
                                && _.ParameterTypeFullName == mappingMethod.ParameterTypeFullName
                                && _.ReturnTypeFullName == mappingMethod.ReturnTypeFullName))
                        {
                            allMappingMethods.Add(mappingMethod);
                        }
                    }
                }

                if (!allMappingMethods.Any())
                {
                    continue;
                }

                var (@namespace, usings) = GetNamespaceAndUsings(classDeclarationSyntax);

                mappingClasses.Add(
                    new(
                        classDeclarationSyntax.Identifier.ToString() + classDeclarationSyntax.TypeParameterList,
                        classDeclarationSyntax.Keyword.ValueText,
                        classDeclarationSyntax.ConstraintClauses.ToString(),
                        @namespace,
                        usings,
                        allMappingMethods));
            }
        }

        return mappingClasses;
    }

    private bool IsMethodSignatureValid(
        IMethodSymbol methodSymbol,
        MethodDeclarationSyntax methodDeclarationSyntax)
    {
        var hasErrors = false;
        if (!methodSymbol.IsPartialDefinition)
        {
            ReportDiagnostic(DiagnosticDescriptors.MappingMethodShouldBePartial, methodDeclarationSyntax.GetLocation());
            hasErrors = true;
        }

        if (methodSymbol.Parameters.Length != 1)
        {
            ReportDiagnostic(DiagnosticDescriptors.MappingMethodShouldHaveSingleParameter, methodDeclarationSyntax.GetLocation());
            hasErrors = true;
        }

        if (methodSymbol.Parameters[0].NullableAnnotation == NullableAnnotation.Annotated)
        {
            ReportDiagnostic(DiagnosticDescriptors.MappingMethodParameterShouldntBeNullable, methodDeclarationSyntax.GetLocation());
            hasErrors = true;
        }

        if (methodSymbol.ReturnsVoid)
        {
            ReportDiagnostic(DiagnosticDescriptors.MappingMethodShouldntReturnVoid, methodDeclarationSyntax.GetLocation());
            hasErrors = true;
        }

        if (methodSymbol.TypeParameters.Length != 0)
        {
            ReportDiagnostic(DiagnosticDescriptors.MappingMethodShouldntBeGeneric, methodDeclarationSyntax.GetLocation());
            hasErrors = true;
        }

        if (!methodSymbol.IsExtensionMethod)
        {
            ReportDiagnostic(DiagnosticDescriptors.MappingMethodShouldBeExtension, methodDeclarationSyntax.GetLocation());
            hasErrors = true;
        }

        return hasErrors;
    }

    private (IReadOnlyCollection<MappingMethod>? MappingMethods, IReadOnlyCollection<TryGetMappingMethodError>? TryGetMappingMethodErrors) TryGetMappingMethod(
        ClassDeclarationSyntax classDeclarationSyntax,
        string methodName,
        ITypeSymbol parameterType,
        string parameterName,
        string parameterSubPath,
        ITypeSymbol returnType,
        SyntaxTokenList modifiers,
        IDictionary<MethodDeclarationSyntax, IMethodSymbol> methodByDeclarationSyntax)
    {
        if (returnType.TypeKind == TypeKind.Enum && parameterType.TypeKind == TypeKind.Enum)
        {
            var (mappingMethod, error) = TryGetEnumMappingMethod(
                methodName,
                parameterType,
                parameterName,
                parameterSubPath,
                returnType,
                modifiers);

            return (mappingMethod is null ? null : new[] { mappingMethod },
                error is null ? null : new [] { error.Value });
        }

        var errors = new List<TryGetMappingMethodError>();
        if (returnType.TypeKind is not (TypeKind.Class or TypeKind.Struct or TypeKind.Interface or TypeKind.Array))
        {
            errors.Add(new(DiagnosticDescriptors.MappingMethodReturnTypeNotSupported, returnType.ToDisplayString()));
        }

        if (parameterType.TypeKind is not (TypeKind.Class or TypeKind.Struct or TypeKind.Interface or TypeKind.Array))
        {
            errors.Add(new(DiagnosticDescriptors.MappingMethodParameterTypeNotSupported, parameterType.ToDisplayString()));
        }

        if (errors.Any())
        {
            return (default, errors);
        }

        return TryGetObjectMappingMethod(
            classDeclarationSyntax,
            methodName,
            parameterType,
            parameterName,
            parameterSubPath,
            returnType,
            modifiers,
            methodByDeclarationSyntax);
    }

    private (IReadOnlyCollection<MappingMethod>? MappingMethods, IReadOnlyCollection<TryGetMappingMethodError>? Error)
        TryGetObjectMappingMethod(
            ClassDeclarationSyntax classDeclarationSyntax,
            string methodName,
            ITypeSymbol parameterType,
            string parameterName,
            string parameterSubPath,
            ITypeSymbol returnType,
            SyntaxTokenList modifiers,
            IDictionary<MethodDeclarationSyntax, IMethodSymbol> methodByDeclarationSyntax)
    {
        var returnTypeMembers = returnType.GetMembers();
        var returnTypeConstructor = default(IMethodSymbol);
        var multipleConstructorsArePresent = false;

        foreach (var returnTypeMember in returnTypeMembers)
        {
            if (returnTypeMember.DeclaredAccessibility == Accessibility.Public
                && returnTypeMember is IMethodSymbol { MethodKind: MethodKind.Constructor } returnTypeMethod
                && returnTypeMethod.Parameters.Length != 0
                && (!returnType.IsRecord
                    || returnTypeMethod.Parameters.Length != 1
                    || !SymbolEqualityComparer.Default.Equals(returnTypeMethod.Parameters[0].Type, returnType)))
            {
                if (returnTypeConstructor is not null)
                {
                    multipleConstructorsArePresent = true;
                }

                returnTypeConstructor = returnTypeMethod;
            }
        }

        if (returnTypeConstructor is null)
        {
            return (default,
                new TryGetMappingMethodError[]
                {
                    new(
                        DiagnosticDescriptors.MappingMethodReturnTypeSuitableConstructorNotFound,
                        returnType.ToDisplayString())
                });
        }

        if (multipleConstructorsArePresent)
        {
            return (default,
                new TryGetMappingMethodError[]
                {
                    new(
                        DiagnosticDescriptors.MappingMethodReturnTypeContainsMultipleSuitableConstructors,
                        returnType.ToDisplayString())
                });
        }

        var mappingMethods = new List<MappingMethod>();

        var parameterTypeMembers = parameterType.GetMembers();
        var mappingProperties = new List<MappingProperty>();

        foreach (var returnTypeConstructorParameter in returnTypeConstructor.Parameters)
        {
            var mappingProperty = default(MappingProperty);
            var dependentMappingMethods = default(IReadOnlyCollection<MappingMethod>);
            var tryGetMappingMethodErrors = default(IReadOnlyCollection<TryGetMappingMethodError>?);
            foreach (var parameterTypeMember in parameterTypeMembers)
            {
                if (parameterTypeMember is not IPropertySymbol parameterTypeProperty
                    || !returnTypeConstructorParameter.Name.Equals(
                        parameterTypeProperty.Name,
                        StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                (mappingProperty, dependentMappingMethods, tryGetMappingMethodErrors) = TryGetMappingProperty(
                    classDeclarationSyntax,
                    methodName,
                    parameterSubPath,
                    parameterTypeProperty.Name,
                    returnTypeConstructorParameter.Type,
                    parameterTypeProperty.Type,
                    modifiers,
                    methodByDeclarationSyntax);

                if (mappingProperty is not null)
                {
                    break;
                }
            }

            if (mappingProperty is null)
            {
                if (tryGetMappingMethodErrors is null)
                {
                    tryGetMappingMethodErrors = new[]
                    {
                        new TryGetMappingMethodError(
                            DiagnosticDescriptors.ConstructorMappingNotFound,
                            returnTypeConstructorParameter.Name,
                            returnType.ToDisplayString(),
                            parameterSubPath),
                    };
                }

                return (default,
                    tryGetMappingMethodErrors);
            }

            mappingProperties.Add(mappingProperty);

            if (dependentMappingMethods is not null)
            {
                mappingMethods.AddRange(dependentMappingMethods);
            }
        }

        mappingMethods.Insert(
            0,
            new(
                methodName,
                modifiers.ToString(),
                parameterType.ToDisplayString(),
                parameterName,
                returnType.ToDisplayString(),
                MappingMethodKind.Object,
                mappingProperties,
                default));

        return (mappingMethods, default);
    }

    private (MappingProperty?, IReadOnlyCollection<MappingMethod>?, IReadOnlyCollection<TryGetMappingMethodError>?) TryGetMappingProperty(
        ClassDeclarationSyntax classDeclarationSyntax,
        string methodName,
        string parameterSubPath,
        string propertyName,
        ITypeSymbol targetType,
        ITypeSymbol sourceType,
        SyntaxTokenList modifiers,
        IDictionary<MethodDeclarationSyntax, IMethodSymbol> methodByDeclarationSyntax)
    {
        var sourceTypeDisplayString = sourceType.ToDisplayString();
        var targetTypeDisplayString = targetType.ToDisplayString();

        // Substring is used to remove "?" symbol. ITypeSymbol.OriginalDefinition doesn't work for generic types e.g. collections
        if (targetTypeDisplayString == sourceTypeDisplayString
            || targetType.NullableAnnotation == NullableAnnotation.Annotated && targetTypeDisplayString.Substring(0, targetTypeDisplayString.Length - 1) == sourceTypeDisplayString)
        {
            return (new(propertyName, MappingPropertyKind.Direct), default, default);
        }

        var shouldUseNullForgivingOperator = sourceType.NullableAnnotation == NullableAnnotation.Annotated;

        var targetItemType = TryGetEnumerableItem(targetType);
        var sourceItemType = TryGetEnumerableItem(sourceType);

        // only one of types is enumerable
        if (targetItemType is null ^ sourceItemType is null)
        {
            return default;
        }

        if (targetItemType is not null)
        {
            targetType = targetItemType;
            sourceType = sourceItemType!;
        }

        if (sourceType.NullableAnnotation == NullableAnnotation.Annotated)
        {
            sourceType = sourceType.OriginalDefinition;
            shouldUseNullForgivingOperator = true;
        }

        if (targetType.NullableAnnotation == NullableAnnotation.Annotated)
        {
            targetType = targetType.OriginalDefinition;
        }

        var matchingExistingMappingMethod = (MethodDeclarationSyntax?)classDeclarationSyntax.Members
            .FirstOrDefault(memberSyntaxDeclaration => memberSyntaxDeclaration is MethodDeclarationSyntax methodDeclarationSyntax
                && methodByDeclarationSyntax.TryGetValue(methodDeclarationSyntax, out var methodSymbol)
                && methodSymbol.IsExtensionMethod
                && methodSymbol.Parameters.Length == 1
                && SymbolEqualityComparer.Default.Equals(methodSymbol.ReturnType, targetType)
                && SymbolEqualityComparer.Default.Equals(methodSymbol.Parameters[0].Type, sourceType));

        if (matchingExistingMappingMethod is not null)
        {
            return (new(
                    propertyName,
                    targetItemType is null ? MappingPropertyKind.SingleItemMapping : MappingPropertyKind.EnumerableMapping,
                    methodByDeclarationSyntax[matchingExistingMappingMethod].Name,
                    shouldUseNullForgivingOperator),
                default,
                default);
        }

        var parameterName = string.Concat(sourceType.Name[0].ToString().ToLower(), sourceType.Name.AsSpan(1));

        var partialModifier = modifiers.FirstOrDefault(_ => _.Text == "partial");
        if (partialModifier != default)
        {
            modifiers = modifiers.Remove(partialModifier);
        }

        var (mappingMethods, tryGetMappingMethodError) = TryGetMappingMethod(
            classDeclarationSyntax,
            methodName,
            sourceType,
            parameterName,
            $"{parameterSubPath}.{propertyName}",
            targetType,
            modifiers,
            methodByDeclarationSyntax);

        if (mappingMethods is null)
        {
            return (default, default, tryGetMappingMethodError);
        }

        return (new(
                propertyName,
                targetItemType is null ? MappingPropertyKind.SingleItemMapping : MappingPropertyKind.EnumerableMapping,
                methodName,
                shouldUseNullForgivingOperator),
            mappingMethods,
            default);
    }

    private ITypeSymbol? TryGetEnumerableItem(ITypeSymbol typeSymbol)
    {
        if (typeSymbol is not INamedTypeSymbol namedTypeSymbol)
        {
            return default;
        }

        if (SymbolEqualityComparer.Default.Equals(namedTypeSymbol.OriginalDefinition, _genericEnumerable))
        {
            return namedTypeSymbol.TypeArguments[0];
        }

        foreach (var @interface in namedTypeSymbol.AllInterfaces)
        {
            if (SymbolEqualityComparer.Default.Equals(@interface.OriginalDefinition, _genericEnumerable))
            {
                return @interface.TypeArguments[0];
            }
        }

        return default;
    }

    private (MappingMethod? MappingMethod, TryGetMappingMethodError? Error) TryGetEnumMappingMethod(
        string methodName,
        ITypeSymbol parameterType,
        string parameterName,
        string parameterSubPath,
        ITypeSymbol returnType,
        SyntaxTokenList modifiers)
    {
        var returnTypeMembers = returnType.GetMembers();
        var parameterTypeMembers = parameterType.GetMembers();
        var enumValues = new List<string>();
        foreach (var returnTypeMember in returnTypeMembers)
        {
            if (!IsEnumValueConstant(returnType, returnTypeMember))
            {
                continue;
            }

            var foundMapping = false;
            foreach (var parameterTypeMember in parameterTypeMembers)
            {
                if (!IsEnumValueConstant(parameterType, parameterTypeMember))
                {
                    continue;
                }

                if (returnTypeMember.Name == parameterTypeMember.Name)
                {
                    foundMapping = true;
                }
            }

            if (!foundMapping)
            {
                return (
                    default,
                    new(
                        DiagnosticDescriptors.EnumMappingNotFound,
                        returnTypeMember.Name,
                        returnType.ToDisplayString(),
                        parameterSubPath));
            }

            enumValues.Add(returnTypeMember.Name);
        }

        var mappingMethod = new MappingMethod(
            methodName,
            modifiers.ToString(),
            parameterType.ToDisplayString(),
            parameterName,
            returnType.ToDisplayString(),
            MappingMethodKind.Enum,
            default,
            enumValues);
        return (mappingMethod, default);

        static bool IsEnumValueConstant(
            ITypeSymbol declaringType,
            ISymbol member)
            => member.DeclaredAccessibility == Accessibility.Public && member.IsStatic
                && member is IFieldSymbol field && SymbolEqualityComparer.Default.Equals(field.Type, declaringType);
    }

    private bool HasGenerateMappingAttribute(IMethodSymbol methodSymbol)
    {
        var attributeDataList = methodSymbol.GetAttributes();

        foreach (var attributeData in attributeDataList)
        {
            if (attributeData.AttributeClass is not null
                && attributeData.AttributeClass.ToDisplayString() == GenerateMappingAttributeFullName)
            {
                return true;
            }
        }

        return false;
    }

    private (string Namespace, string Usings) GetNamespaceAndUsings(ClassDeclarationSyntax classDeclarationSyntax)
    {
        var namespaces = new List<string>();
        var usings = new SyntaxList<UsingDirectiveSyntax>();

        var parentSyntax = classDeclarationSyntax.Parent;
        while (parentSyntax is not null)
        {
            if (parentSyntax is BaseNamespaceDeclarationSyntax baseNamespaceDeclarationSyntax)
            {
                namespaces.Add(baseNamespaceDeclarationSyntax.Name.ToString());
                usings = usings.AddRange(baseNamespaceDeclarationSyntax.Usings);
            } else if (parentSyntax is CompilationUnitSyntax compilationUnitSyntax)
            {
                usings = usings.AddRange(compilationUnitSyntax.Usings);
            }

            parentSyntax = parentSyntax.Parent;
        }

        return (string.Join('.', namespaces), usings.ToFullString());
    }

    private void ReportDiagnostic(
        DiagnosticDescriptor diagnosticDescriptor,
        Location? location,
        params object?[]? messageArgs)
    {
        _reportDiagnostic(Diagnostic.Create(diagnosticDescriptor, location, messageArgs));
    }

    private record struct TryGetMappingMethodError(
        DiagnosticDescriptor DiagnosticDescriptor,
        string? Argument0 = default,
        string? Argument1 = default,
        string? Argument2 = default);
}