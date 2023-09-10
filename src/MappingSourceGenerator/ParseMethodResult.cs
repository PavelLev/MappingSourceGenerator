using System.Collections.Immutable;
using Microsoft.CodeAnalysis;

namespace MappingSourceGenerator;

public record ParseMethodResult(
    IReadOnlyCollection<MappingMethod> MappingMethods,
    IReadOnlyCollection<Diagnostic> Diagnostics);