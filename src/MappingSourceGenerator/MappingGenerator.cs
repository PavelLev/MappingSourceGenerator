using System.Collections.Immutable;
using System.Text;
using MappingSourceGenerator.Markers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace MappingSourceGenerator;

[Generator(LanguageNames.CSharp)]
public class MappingGenerator : IIncrementalGenerator
{
    private static readonly string GenerateMappingAttributeFullName = typeof(GenerateMappingAttribute).FullName!;
    private static readonly string GenerateMappingAttributeName = typeof(GenerateMappingAttribute).Name!;
    private static readonly string GenerateMappingAttributeNameWithoutSuffix;
    private static readonly IReadOnlyList<string> GenerateMappingAttributeNamespaceNames;

    private readonly IParser _parser;
    private readonly IEmitter _emitter;

    static MappingGenerator()
    {
        GenerateMappingAttributeNameWithoutSuffix = GenerateMappingAttributeName.Substring(
            0,
            GenerateMappingAttributeName.Length - "Attribute".Length);

        GenerateMappingAttributeNamespaceNames = typeof(GenerateMappingAttribute).Namespace!
            .Split('.');
    }

    // Default constructor for Roslyn
    public MappingGenerator()
        : this(new Parser(), new ManualInterpolationEmitter())
    {
    }

    // Constructor for testing allowing using custom parser and emitter
    public MappingGenerator(
        IParser parser,
        IEmitter emitter)
    {
        _parser = parser;
        _emitter = emitter;
    }
    
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        // System.Diagnostics.Debugger.Launch();

        // var markedMethodSymbolsProvider = ForAttributeWithMetadataName(context);
        var markedMethodSymbolsProvider = CustomMethodSymbolsWithAttribute(context);
        
        var parseMethodResultProvider = markedMethodSymbolsProvider
            .Select((
                markedMethods,
                cancellationToken) => _parser.GetMapperClasses(markedMethods, cancellationToken))
            .WithComparer(ParseMethodResultEqualityComparer.Default);

        context.RegisterSourceOutput(
            parseMethodResultProvider, 
            (sourceProductionContext, source) => Execute(source, sourceProductionContext));
    }

    private void Execute(ParseMethodResult parseMethodResult, SourceProductionContext context)
    {
        foreach (var diagnostic in parseMethodResult.Diagnostics)
        {
            context.ReportDiagnostic(diagnostic);
        }

        if (!parseMethodResult.MappingMethods.Any())
        {
            return;
        }
        
        _emitter.Emit(
            context.AddSource,
            parseMethodResult.MappingMethods,
            context.CancellationToken);
    }

    // Built-in way to find symbols marked with an attribute.
    // Since it needs to support all c# features and all attributes it's 10% slower than
    // CustomMethodSymbolsWithAttribute implementation.
    // Unless the custom implementation doesn't cover some cases I'd suggest to keep it.
    private static IncrementalValueProvider<ImmutableArray<IMethodSymbol>> ForAttributeWithMetadataName(IncrementalGeneratorInitializationContext context)
    {
        return context.SyntaxProvider
            .ForAttributeWithMetadataName(
                GenerateMappingAttributeFullName,
                static (
                    syntaxNode,
                    cancellationToken) => syntaxNode is MethodDeclarationSyntax,
                static (
                    generatorAttributeSyntaxContext,
                    cancellationToken) => (IMethodSymbol)generatorAttributeSyntaxContext.TargetSymbol)
            .Collect();
    }

    // The custom implementation of ForAttributeWithMetadataName method.
    private static IncrementalValueProvider<ImmutableArray<IMethodSymbol>> CustomMethodSymbolsWithAttribute(
        IncrementalGeneratorInitializationContext context)
    {
        return context.SyntaxProvider
            .CreateSyntaxProvider(
                static (syntaxNode, cancellationToken) 
                    => syntaxNode is MethodDeclarationSyntax methodDeclarationSyntax
                        && methodDeclarationSyntax.AttributeLists.Any(
                            static attributeListSyntax => attributeListSyntax.Attributes.Any(
                                static attributeSyntax =>
                                {
                                    var attributeName = attributeSyntax.Name.ToString();
                                    return attributeName == GenerateMappingAttributeName 
                                        || attributeName == GenerateMappingAttributeNameWithoutSuffix;
                                })),
                (generatorSyntaxContext, cancellationToken) 
                    => (IMethodSymbol)generatorSyntaxContext.SemanticModel
                        .GetDeclaredSymbol(generatorSyntaxContext.Node, cancellationToken)!)
            .Where(methodSymbol 
                => methodSymbol.GetAttributes()
                    .Any(attribute 
                        => attribute.AttributeClass is not null 
                            && DoesAttributeNamespaceMatch(
                                attribute.AttributeClass,
                                GenerateMappingAttributeNamespaceNames.Count - 1)))
            .Collect();

        bool DoesAttributeNamespaceMatch(
            ISymbol symbol,
            int index)
        {
            // we've compared all symbol containing names
            if (symbol.ContainingSymbol is INamespaceSymbol { IsGlobalNamespace: true })
            {
                // GenerateMappingAttribute can have unchecked containing names
                return index < 0;
            }

            if (index < 0)
            {
                return false;
            }

            var expectedName = GenerateMappingAttributeNamespaceNames[index];

            return symbol.ContainingSymbol.Name == expectedName
                && DoesAttributeNamespaceMatch(symbol.ContainingSymbol, index - 1);
        }
    }
}