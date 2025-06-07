using Microsoft.CodeAnalysis;

namespace MappingSourceGenerator.Benchmarks;

[Generator(LanguageNames.CSharp)]
public sealed class NullCSharpIncrementalGenerator : IIncrementalGenerator
{
	public void Initialize(IncrementalGeneratorInitializationContext context)
	{
	}
}
