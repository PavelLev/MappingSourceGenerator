using System.Collections.Immutable;
using Microsoft.CodeAnalysis;

namespace MappingSourceGenerator;

public interface IParser
{
    ParseMethodResult GetMapperClasses(
        ImmutableArray<IMethodSymbol> markedMethods,
        CancellationToken cancellationToken);
}