using System.Collections.Immutable;
using Microsoft.CodeAnalysis;

namespace MappingSourceGenerator.Benchmarks;

public class ParameterMemorizingParser : IParser
{
    private readonly IParser _parser;

    public ParameterMemorizingParser(IParser parser)
    {
        _parser = parser;
    }

    public ParseMethodResult GetMapperClasses(
        ImmutableArray<IMethodSymbol> markedMethods,
        CancellationToken cancellationToken)
    {
        MarkedMethods = markedMethods;
        var parseMethodResult = _parser.GetMapperClasses(markedMethods, cancellationToken);
        MappingMethods = parseMethodResult.MappingMethods;
        return parseMethodResult;
    }
    
    public ImmutableArray<IMethodSymbol>? MarkedMethods { get; private set; }
    
    public IReadOnlyCollection<MappingMethod>? MappingMethods { get; private set; }
}