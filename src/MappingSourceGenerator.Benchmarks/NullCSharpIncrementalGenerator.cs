using Microsoft.CodeAnalysis;

namespace F0.CodeAnalysis.CSharp.Benchmarking;

[Generator(LanguageNames.CSharp)]
public sealed class NullCSharpIncrementalGenerator : IIncrementalGenerator
{
	public void Initialize(IncrementalGeneratorInitializationContext context)
	{
	}
}
