using System.Collections.Immutable;
using System.Runtime.InteropServices;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace MappingSourceGenerator;

public class Parser : IParser
{
    private readonly Dictionary<INamespaceOrTypeSymbol, ImmutableArray<ISymbol>> _membersBySymbol = new(SymbolEqualityComparer.Default);
    private readonly Dictionary<IMethodSymbol, ImmutableArray<IParameterSymbol>> _parametersByMethodSymbol = new(SymbolEqualityComparer.Default);
    private readonly Dictionary<IMethodSymbol, Location> _locationByMethodSymbol = new(SymbolEqualityComparer.Default);
    private readonly Dictionary<MappingMethod, (ITypeSymbol ClassType, ITypeSymbol ParameterType, ITypeSymbol ReturnType)> _symbolsByMappingMethod = new();

    public ParseMethodResult GetMapperClasses(
        ImmutableArray<IMethodSymbol> markedMethods,
        CancellationToken cancellationToken)
    {
        if (!markedMethods.Any())
        {
            return new(
                Array.Empty<MappingMethod>(),
                Array.Empty<Diagnostic>());
        }
        
        var mappingMethods = new List<MappingMethod>(markedMethods.Length);
        var diagnostics = new List<Diagnostic>();
        
        var currentMappingClass = markedMethods.First().ContainingType!;
        
        foreach (var markedMethod in markedMethods)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var markedMethodParameters = GetParametersWithCaching(markedMethod);
            var hasErrors = IsMethodSignatureValid(
                markedMethod,
                markedMethodParameters,
                diagnostics);

            if (hasErrors)
            {
                continue;
            }

            var nextMappingClass = markedMethod.ContainingType;
            if (!SymbolEqualityComparer.Default.Equals(currentMappingClass, nextMappingClass))
            {
                currentMappingClass = nextMappingClass;
            }

            var parameter = markedMethodParameters[0];
            TryGetMappingMethod(
                mappingMethods,
                diagnostics,
                markedMethod,
                parameter.Type,
                parameter.Name,
                parameter.Name,
                markedMethod.ReturnType,
                markedMethod.DeclaredAccessibility,
                true,
                currentMappingClass);
        }

        _membersBySymbol.Clear();
        _parametersByMethodSymbol.Clear();
        _parametersByMethodSymbol.Clear();
        _symbolsByMappingMethod.Clear();
        
        return new(
            mappingMethods,
            diagnostics);
    }

    private bool IsMethodSignatureValid(
        IMethodSymbol method,
        ImmutableArray<IParameterSymbol> methodParameters,
        ICollection<Diagnostic> diagnostics)
    {
        var hasErrors = false;
        if (!method.IsPartialDefinition)
        {
            diagnostics.Add(
                Diagnostic.Create(DiagnosticDescriptors.MappingMethodShouldBePartial, GetLocationWithCaching(method)));
            hasErrors = true;
        }

        if (methodParameters.Length != 1)
        {
            diagnostics.Add(
                Diagnostic.Create(DiagnosticDescriptors.MappingMethodShouldHaveSingleParameter, GetLocationWithCaching(method)));
            hasErrors = true;
        }

        if (methodParameters[0].NullableAnnotation == NullableAnnotation.Annotated)
        {
            diagnostics.Add(
                Diagnostic.Create(DiagnosticDescriptors.MappingMethodParameterShouldNotBeNullable, GetLocationWithCaching(method)));
            hasErrors = true;
        }

        if (method.ReturnsVoid)
        {
            diagnostics.Add(
                Diagnostic.Create(DiagnosticDescriptors.MappingMethodShouldNotReturnVoid, GetLocationWithCaching(method)));
            hasErrors = true;
        }

        if (method.IsGenericMethod)
        {
            diagnostics.Add(
                Diagnostic.Create(DiagnosticDescriptors.MappingMethodShouldNotBeGeneric, GetLocationWithCaching(method)));
            hasErrors = true;
        }

        if (!method.IsExtensionMethod)
        {
            diagnostics.Add(
                Diagnostic.Create(DiagnosticDescriptors.MappingMethodShouldBeExtension, GetLocationWithCaching(method)));
            hasErrors = true;
        }

        if (method.ContainingType.ContainingSymbol is not INamespaceSymbol)
        {
            diagnostics.Add(
                Diagnostic.Create(DiagnosticDescriptors.MappingMethodContainingClassShouldNotBeNested, GetLocationWithCaching(method)));
            hasErrors = true;
        }

        return hasErrors;
    }

    private bool TryGetMappingMethod(
        ICollection<MappingMethod> mappingMethods,
        ICollection<Diagnostic> diagnostics,
        IMethodSymbol markedMethod,
        ITypeSymbol parameterType,
        string parameterName,
        string parameterSubPath,
        ITypeSymbol returnType,
        Accessibility accessibility,
        bool isPartial,
        INamedTypeSymbol mappingClass)
    {
        if (returnType.TypeKind == TypeKind.Enum && parameterType.TypeKind == TypeKind.Enum)
        {
            return TryGetEnumMappingMethod(
                mappingMethods,
                diagnostics,
                markedMethod,
                parameterType,
                parameterName,
                parameterSubPath,
                returnType,
                accessibility,
                isPartial,
                mappingClass);
        }

        var hasErrors = false;
        if (returnType.TypeKind is not (TypeKind.Class or TypeKind.Struct))
        {
            diagnostics.Add(
                Diagnostic.Create(
                    DiagnosticDescriptors.MappingMethodReturnTypeNotSupported, 
                    GetLocationWithCaching(markedMethod),
                    returnType.ToDisplayString()));
            hasErrors = true;
        }

        if (parameterType.TypeKind is not (TypeKind.Class or TypeKind.Struct ))
        {
            diagnostics.Add(
                Diagnostic.Create(
                    DiagnosticDescriptors.MappingMethodParameterTypeNotSupported, 
                    GetLocationWithCaching(markedMethod),
                    parameterType.ToDisplayString(),
                    parameterType.TypeKind.ToString()));
            hasErrors = true;
        }

        if (hasErrors)
        {
            return false;
        }

        return TryGetObjectMappingMethod(
            mappingMethods,
            diagnostics,
            markedMethod,
            parameterType,
            parameterName,
            parameterSubPath,
            returnType,
            accessibility,
            isPartial,
            mappingClass);
    }

    private bool TryGetObjectMappingMethod(
        ICollection<MappingMethod> mappingMethods,
        ICollection<Diagnostic> diagnostics,
        IMethodSymbol markedMethod,
        ITypeSymbol parameterType,
        string parameterName,
        string parameterSubPath,
        ITypeSymbol returnType,
        Accessibility accessibility,
        bool isPartial,
        INamedTypeSymbol mappingClass)
    {
        var returnTypeMembers = GetMembersWithCaching(returnType);
        var returnTypeConstructor = default(IMethodSymbol);
        var returnTypeConstructorParameters = default(ImmutableArray<IParameterSymbol>);
        var multipleConstructorsArePresent = false;

        foreach (var returnTypeMember in returnTypeMembers)
        {
            if (returnTypeMember.DeclaredAccessibility != Accessibility.Public
                || returnTypeMember is not IMethodSymbol { MethodKind: MethodKind.Constructor } returnTypeMethod)
            {
                continue;
            }

            var returnTypeMethodParameters = GetParametersWithCaching(returnTypeMethod);
            
            if (returnTypeMethodParameters.Length != 0 // non-default constructor
                && (!returnType.IsRecord // main constructor of a record
                    || returnTypeMethodParameters.Length != 1
                    || !SymbolEqualityComparer.Default.Equals(returnTypeMethodParameters[0].Type, returnType)))
            {
                if (returnTypeConstructor is not null)
                {
                    multipleConstructorsArePresent = true;
                }

                returnTypeConstructor = returnTypeMethod;
                returnTypeConstructorParameters = returnTypeMethodParameters;
            }
        }

        if (returnTypeConstructor is null)
        {
            diagnostics.Add(
                Diagnostic.Create(
                    DiagnosticDescriptors.MappingMethodReturnTypeSuitableConstructorNotFound,
                    GetLocationWithCaching(markedMethod),
                    returnType.ToDisplayString()));
            return false;
        }

        if (multipleConstructorsArePresent)
        {
            diagnostics.Add(
                Diagnostic.Create(
                    DiagnosticDescriptors.MappingMethodReturnTypeContainsMultipleSuitableConstructors,
                    GetLocationWithCaching(markedMethod),
                    returnType.ToDisplayString()));
            return false;
        }
        var parameterTypeMembers = GetMembersWithCaching(parameterType);
        var mappingProperties = ImmutableArray.CreateBuilder<MappingProperty>(returnTypeConstructorParameters.Length);

        foreach (var returnTypeConstructorParameter in returnTypeConstructorParameters)
        {
            var correspondingParameterTypeProperty = default(IPropertySymbol?);
            foreach (var parameterTypeMember in parameterTypeMembers)
            {
                if (parameterTypeMember is not IPropertySymbol parameterTypeProperty
                    || !returnTypeConstructorParameter.Name.Equals(
                        parameterTypeProperty.Name,
                        StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                correspondingParameterTypeProperty = parameterTypeProperty;
                break;
            }

            if (correspondingParameterTypeProperty is null)
            {
                diagnostics.Add(
                    Diagnostic.Create(
                        DiagnosticDescriptors.ConstructorMappingNotFound,
                        GetLocationWithCaching(markedMethod),
                        returnTypeConstructorParameter.Name,
                        returnType.ToDisplayString(),
                        parameterType.ToDisplayString()));
                continue;
            }

            var mappingProperty = TryGetMappingProperty(
                mappingMethods,
                diagnostics,
                markedMethod,
                parameterSubPath,
                correspondingParameterTypeProperty.Name,
                returnTypeConstructorParameter.Type,
                correspondingParameterTypeProperty.Type,
                accessibility,
                mappingClass);

            if (mappingProperty is not null)
            {
                mappingProperties.Add(mappingProperty);
            }
        }

        if (mappingProperties.Count != returnTypeConstructorParameters.Length)
        {
            return false;
        }

        var mappingMethod = new MappingMethod(
            mappingClass.Name,
            GetContainingNames(mappingClass),
            markedMethod.Name,
            accessibility,
            isPartial,
            parameterType.Name,
            GetContainingNames(parameterType),
            parameterName,
            returnType.Name,
            GetContainingNames(returnType),
            MappingMethodKind.Object,
            mappingProperties,
            default);
        _symbolsByMappingMethod.Add(mappingMethod, (mappingClass, parameterType, returnType));
        mappingMethods.Add(
            mappingMethod);

        return true;
    }

    private MappingProperty? TryGetMappingProperty(
        ICollection<MappingMethod> mappingMethods,
        ICollection<Diagnostic> diagnostics,
        IMethodSymbol markedMethod,
        string parameterSubPath,
        string propertyName,
        ITypeSymbol targetType,
        ITypeSymbol sourceType,
        Accessibility accessibility,
        INamedTypeSymbol mappingClass)
    {
        if (IsDeFactoNullable(sourceType) && !IsDeFactoNullable(targetType))
        {
            diagnostics.Add(
                Diagnostic.Create(
                    DiagnosticDescriptors.NullableToNonNullableMapping,
                    GetLocationWithCaching(markedMethod),
                    propertyName,
                    sourceType.ToDisplayString(),
                    targetType.ToDisplayString(),
                    parameterSubPath));
            return default;
        }

        var (targetItemType, isTargetCollectionList, targetCollectionType) = TryGetEnumerableItem(targetType);
        var (sourceItemType, _, sourceCollectionType) = TryGetEnumerableItem(sourceType);
        
        if (sourceItemType?.NullableAnnotation == NullableAnnotation.Annotated)
        {
            diagnostics.Add(
                Diagnostic.Create(
                    DiagnosticDescriptors.EnumerableWithNullableItemMappingNotSupported,
                    GetLocationWithCaching(markedMethod),
                    propertyName,
                    sourceType.ToDisplayString(),
                    parameterSubPath));
            return default;
        }

        if (SymbolEqualityComparer.Default.Equals(targetType, sourceType))
        {
            return new(propertyName, MappingPropertyKind.Direct, null, false, false);
        }

        if (sourceType.IsValueType
            && targetType is { IsValueType: true, OriginalDefinition.SpecialType: SpecialType.System_Nullable_T }
                and INamedTypeSymbol namedTargetType
            && SymbolEqualityComparer.Default.Equals(namedTargetType.TypeArguments[0], sourceType))
        {
            return new(propertyName, MappingPropertyKind.Direct, null, false, false);
        }

        if (SymbolEqualityComparer.Default.Equals(sourceItemType, targetItemType)
            && AreCollectionTypesCompatible(sourceCollectionType, targetCollectionType))
        {
            return new(propertyName, MappingPropertyKind.Direct, null, false, false);
        }
        
        var shouldUseNullConditionalOperator = sourceType.NullableAnnotation == NullableAnnotation.Annotated;

        var existingSingleItemMapMethodName = GetExistingMappingMethodOrDefault(
            this,
            GetMembersWithCaching(mappingClass),
            markedMethod,
            targetType,
            sourceType);
        
        if (existingSingleItemMapMethodName is not null)
        {
            return new(
                propertyName,
                MappingPropertyKind.SingleItemMapping,
                existingSingleItemMapMethodName,
                shouldUseNullConditionalOperator,
                false);
        }

        // only one of types is enumerable
        if (sourceItemType is not null && targetItemType is null)
        {
            diagnostics.Add(
                Diagnostic.Create(
                    DiagnosticDescriptors.EnumerableToNonEnumerableMappingNotSupported,
                    GetLocationWithCaching(markedMethod),
                    propertyName,
                    sourceType.ToDisplayString(),
                    targetType.ToDisplayString(),
                    parameterSubPath));
            return default;
        }
        
        if (sourceItemType is null && targetItemType is not null)
        {
            diagnostics.Add(
                Diagnostic.Create(
                    DiagnosticDescriptors.NonEnumerableToEnumerableMappingNotSupported,
                    GetLocationWithCaching(markedMethod),
                    propertyName,
                    sourceType.ToDisplayString(),
                    targetType.ToDisplayString(),
                    parameterSubPath));
            return default;
        }

        if (targetItemType is not null)
        {
            var existingEnumerableMapMethodName = GetExistingMappingMethodOrDefault(
                this,
                GetMembersWithCaching(mappingClass),
                markedMethod,
                targetItemType,
                sourceItemType!);
            
            if (existingEnumerableMapMethodName is not null)
            {
                return new(
                    propertyName,
                    MappingPropertyKind.EnumerableMapping,
                    existingEnumerableMapMethodName,
                    shouldUseNullConditionalOperator,
                    isTargetCollectionList);
            }
            
            targetType = targetItemType;
            sourceType = sourceItemType!;
        }

        RemoveNullability(ref sourceType);
        RemoveNullability(ref targetType);

        if (MappingMethodAlreadyGenerated(_symbolsByMappingMethod, mappingMethods, markedMethod.Name, mappingClass, targetType, sourceType))
        {
            return new(
                propertyName,
                targetItemType is null ? MappingPropertyKind.SingleItemMapping : MappingPropertyKind.EnumerableMapping,
                markedMethod.Name,
                shouldUseNullConditionalOperator,
                isTargetCollectionList);
        }

        var isMappingFound = TryGetMappingMethod(
            mappingMethods,
            diagnostics,
            markedMethod,
            sourceType,
            sourceType.Name,
            $"{parameterSubPath}.{propertyName}",
            targetType,
            accessibility,
            false,
            mappingClass);

        if (!isMappingFound)
        {
            return null;
        }
        
        return new(
            propertyName,
            targetItemType is null ? MappingPropertyKind.SingleItemMapping : MappingPropertyKind.EnumerableMapping,
            markedMethod.Name,
            shouldUseNullConditionalOperator,
            isTargetCollectionList);

        static bool IsDeFactoNullable(ITypeSymbol type)
            => type.NullableAnnotation is NullableAnnotation.Annotated or NullableAnnotation.None;

        static void RemoveNullability(ref ITypeSymbol typeSymbol)
        {
            if (typeSymbol.NullableAnnotation == NullableAnnotation.Annotated)
            {
                if (typeSymbol is { IsValueType: true, OriginalDefinition.SpecialType: SpecialType.System_Nullable_T } 
                    and INamedTypeSymbol namedTypeSymbol)
                {
                    typeSymbol = namedTypeSymbol.TypeArguments[0];
                }
                else
                {
                    typeSymbol = typeSymbol.OriginalDefinition;
                }
            }
        }

        static bool AreCollectionTypesCompatible(CollectionType? sourceCollectionType, CollectionType? targetCollectionType)
        {
            return (sourceCollectionType, targetCollectionType) switch
            {
                (CollectionType.IReadOnlyCollection, CollectionType.IEnumerable) => true,
                (CollectionType.IReadOnlyList, CollectionType.IEnumerable) => true,
                (CollectionType.IReadOnlyList, CollectionType.IReadOnlyCollection) => true,
                (CollectionType.ICollection, CollectionType.IEnumerable) => true,
                (CollectionType.IList, CollectionType.IEnumerable) => true,
                (CollectionType.IList, CollectionType.ICollection) => true,
                (CollectionType.Array, CollectionType.IEnumerable) => true,
                (CollectionType.Array, CollectionType.IReadOnlyCollection) => true,
                (CollectionType.Array, CollectionType.IReadOnlyList) => true,
                (CollectionType.List, CollectionType.IEnumerable) => true,
                (CollectionType.List, CollectionType.IReadOnlyCollection) => true,
                (CollectionType.List, CollectionType.IReadOnlyList) => true,
                (CollectionType.List, CollectionType.ICollection) => true,
                (CollectionType.List, CollectionType.IList) => true,
                _ => false,
            };
        }

        static string? GetExistingMappingMethodOrDefault(
            Parser parser,
            ImmutableArray<ISymbol> mappingClassMembers,
            IMethodSymbol markedMethod,
            ITypeSymbol targetType,
            ITypeSymbol sourceType)
        {
            // Enumerable.Any() transformed into foreach to avoid closure allocations
            // (around 6% of Parser.GetMapperClasses allocations at the moment of optimization)
            foreach (var member in mappingClassMembers)
            {
                if (member is not IMethodSymbol { IsExtensionMethod: true } method)
                {
                    continue;
                }

                var methodParameters = parser.GetParametersWithCaching(method);

                var isMethodMatching = methodParameters.Length == 1
                    && CanMapMethodBeUsed(markedMethod, method.Name)
                    && SymbolEqualityComparer.Default.Equals(method.ReturnType, targetType)
                    && SymbolEqualityComparer.Default.Equals(methodParameters[0].Type, sourceType);

                if (isMethodMatching)
                {
                    return method.Name;
                }
            }

            return null;
        }
        
        static bool CanMapMethodBeUsed(
            IMethodSymbol markedMethod,
            string targetMethodName)
        {
            if (markedMethod.Name == targetMethodName)
            {
                return true;
            }

            var attribute = markedMethod.GetAttributes().First();
            var usableMapMethodNames = attribute.ConstructorArguments.FirstOrDefault();
            
            foreach (var typedConstant in usableMapMethodNames.Values)
            {
                if ((string?)typedConstant.Value == targetMethodName)
                {
                    return true;
                }
            }

            return false;
        }

        static bool MappingMethodAlreadyGenerated(
            IReadOnlyDictionary<MappingMethod, (ITypeSymbol ClassType, ITypeSymbol ParameterType, ITypeSymbol ReturnType)> symbolsByMappingMethod,
            IEnumerable<MappingMethod> mappingMethods,
            string methodName,
            ITypeSymbol containingType,
            ITypeSymbol targetType,
            ITypeSymbol sourceType)
        {
            // Enumerable.Any() transformed into foreach to avoid closure allocations
            // (around 6% of Parser.GetMapperClasses allocations at the moment of optimization)
            foreach (var mappingMethod in mappingMethods)
            {
                // We don't want to use CanMapMethodBeUsed here, since we'd generate different result
                // for different map methods declaration order
                if (mappingMethod.Name != methodName)
                {
                    continue;
                }
                
                var (classType, parameterType, returnType) = symbolsByMappingMethod[mappingMethod];

                var isMethodMatching = SymbolEqualityComparer.Default.Equals(classType, containingType)
                    && SymbolEqualityComparer.Default.Equals(sourceType, parameterType)
                    && SymbolEqualityComparer.Default.Equals(targetType, returnType);

                if (isMethodMatching)
                {
                    return true;
                }
            }

            return false;
        }
    }

    private (ITypeSymbol? ItemType, bool IsList, CollectionType? CollectionType) TryGetEnumerableItem(
        ITypeSymbol typeSymbol)
    {
        if (typeSymbol is IArrayTypeSymbol arrayTypeSymbol)
        {
            return (arrayTypeSymbol.ElementType, false, CollectionType.Array);
        }

        var specialType = typeSymbol.OriginalDefinition.SpecialType;
        
        if (specialType is SpecialType.System_Collections_Generic_IEnumerable_T
            or SpecialType.System_Collections_Generic_ICollection_T
            or SpecialType.System_Collections_Generic_IList_T
            or SpecialType.System_Collections_Generic_IReadOnlyCollection_T
            or SpecialType.System_Collections_Generic_IReadOnlyList_T)
        {
            return (((INamedTypeSymbol)typeSymbol).TypeArguments[0], false, Map(specialType));
        }

        // dirty hack to support System.Collections.Generic.List<T> without thorough checks
        if (typeSymbol is INamedTypeSymbol { Name: "List", TypeArguments.Length: 1 } namedTypeSymbol)
        {
            return (namedTypeSymbol.TypeArguments[0], true, CollectionType.List);
        }

        return default;

        static CollectionType? Map(SpecialType specialType)
            => specialType switch
            {
                SpecialType.System_Collections_Generic_IEnumerable_T => CollectionType.IEnumerable,
                SpecialType.System_Collections_Generic_ICollection_T => CollectionType.ICollection,
                SpecialType.System_Collections_Generic_IList_T => CollectionType.IList,
                SpecialType.System_Collections_Generic_IReadOnlyCollection_T => CollectionType.IReadOnlyCollection,
                SpecialType.System_Collections_Generic_IReadOnlyList_T => CollectionType.IReadOnlyList,
                _ => null
            };
    }

    private bool TryGetEnumMappingMethod(
        ICollection<MappingMethod> mappingMethods,
        ICollection<Diagnostic> diagnostics,
        IMethodSymbol markedMethod,
        ITypeSymbol parameterType,
        string parameterName,
        string parameterSubPath,
        ITypeSymbol returnType,
        Accessibility accessibility,
        bool isPartial,
        INamedTypeSymbol mappingClass)
    {
        var returnTypeMembers = GetMembersWithCaching(returnType);
        var parameterTypeMembers = GetMembersWithCaching(parameterType);
        // enum members consist of a default constructor and constants, so we need subtract 1 to get count of constants
        var enumValuesCount = parameterTypeMembers.Length - 1; 
        var enumValues = new List<string>(enumValuesCount);
        foreach (var parameterTypeMember in parameterTypeMembers)
        {
            if (!IsEnumValueConstant(parameterType, parameterTypeMember))
            {
                continue;
            }

            var foundMapping = false;
            foreach (var returnTypeMember in returnTypeMembers)
            {
                if (!IsEnumValueConstant(returnType, returnTypeMember))
                {
                    continue;
                }

                if (returnTypeMember.Name == parameterTypeMember.Name)
                {
                    foundMapping = true;
                    break;
                }
            }

            if (!foundMapping)
            {
                diagnostics.Add(
                    Diagnostic.Create(
                        DiagnosticDescriptors.EnumMappingNotFound,
                        GetLocationWithCaching(markedMethod),
                        parameterTypeMember.Name,
                        returnType.ToDisplayString(),
                        parameterSubPath));
                continue;
            }

            enumValues.Add(parameterTypeMember.Name);
        }

        if (enumValues.Count != enumValuesCount)
        {
            return false;
        }

        var mappingMethod = new MappingMethod(
            mappingClass.Name,
            GetContainingNames(mappingClass),
            markedMethod.Name,
            accessibility,
            isPartial,
            parameterType.Name,
            GetContainingNames(parameterType),
            parameterName,
            returnType.Name,
            GetContainingNames(returnType),
            MappingMethodKind.Enum,
            default,
            enumValues);
        _symbolsByMappingMethod.Add(mappingMethod, (mappingClass, parameterType, returnType));
        mappingMethods.Add(
            mappingMethod);
        return true;

        static bool IsEnumValueConstant(
            ITypeSymbol declaringType,
            ISymbol member)
            => member.DeclaredAccessibility == Accessibility.Public && member.IsStatic
                && member is IFieldSymbol field && SymbolEqualityComparer.Default.Equals(field.Type, declaringType);
    }

    private IReadOnlyList<string> GetContainingNames(ISymbol symbol)
    {
        return GetContainingNamesRecursive(symbol, 0);

        static string[] GetContainingNamesRecursive(
            ISymbol symbol,
            int count)
        {
            if (symbol.ContainingSymbol is INamespaceSymbol { IsGlobalNamespace: true })
            {
                return new string[count];
            }

            var containingNames = GetContainingNamesRecursive(
                symbol.ContainingSymbol,
                count + 1);

            containingNames[containingNames.Length - count - 1] = symbol.ContainingSymbol.Name;

            return containingNames;
        }
    }

    private ImmutableArray<ISymbol> GetMembersWithCaching(INamespaceOrTypeSymbol namespaceOrType)
    {
        if (!_membersBySymbol.TryGetValue(namespaceOrType, out var members))
        {
            members = namespaceOrType.GetMembers();
            _membersBySymbol.Add(namespaceOrType, members);
        }

        return members;
    }

    private ImmutableArray<IParameterSymbol> GetParametersWithCaching(IMethodSymbol methodSymbol)
    {
        if (!_parametersByMethodSymbol.TryGetValue(methodSymbol, out var parameters))
        {
            parameters = methodSymbol.Parameters;
            _parametersByMethodSymbol.Add(methodSymbol, parameters);
        }

        return parameters;
    }

    private Location GetLocationWithCaching(IMethodSymbol method)
    {
        if (!_locationByMethodSymbol.TryGetValue(method, out var location))
        {
            location = method.DeclaringSyntaxReferences.First().GetSyntax().GetLocation();
            _locationByMethodSymbol.Add(method, location);
        }

        return location;
    }

    private enum CollectionType
    {
        Array,
        IEnumerable,
        IReadOnlyCollection,
        IReadOnlyList,
        ICollection,
        IList,
        List
    }
}