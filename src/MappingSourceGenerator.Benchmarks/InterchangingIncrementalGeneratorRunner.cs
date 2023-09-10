using F0.CodeAnalysis.CSharp.Benchmarking;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace MappingSourceGenerator.Benchmarks;

public class InterchangingIncrementalGeneratorRunner
{
    private GeneratorDriver _generatorDriver;
    private Compilation _currentCompilation;
    private Compilation _nextCompilation;
    
    public InterchangingIncrementalGeneratorRunner(
        IIncrementalGenerator incrementalGenerator,
        string source,
        IReadOnlyCollection<MetadataReference> metadataReferences)
    {
        _generatorDriver = CSharpGeneratorDriver.Create(incrementalGenerator);

        _currentCompilation = CreateCompilation();
        _nextCompilation = CreateCompilation();

        return;

        Compilation CreateCompilation()
        {
            const string assemblyName = "CompilerGeneratedCompilation";

            var syntaxTree = CSharpSyntaxTree.ParseText(source!);
            
            return CSharpCompilation.Create(assemblyName, new[] {syntaxTree}, metadataReferences);
        }
    }
    
    public object Invoke()
    {
        _ = _generatorDriver.RunGenerators(_currentCompilation, CancellationToken.None);

        return null!;
    }

    public object InvokeWithMemoization()
    {
        _generatorDriver = _generatorDriver.RunGenerators(_currentCompilation, CancellationToken.None);

        (_currentCompilation, _nextCompilation) = (_nextCompilation, _currentCompilation);

        return null!;
    }
}