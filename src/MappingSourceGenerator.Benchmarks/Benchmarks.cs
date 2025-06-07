using System.Collections.Immutable;
using System.Diagnostics;
using System.Reflection;
using BenchmarkDotNet.Attributes;
using MappingSourceGenerator.Markers;
using Microsoft.CodeAnalysis;

namespace MappingSourceGenerator.Benchmarks;

[MemoryDiagnoser]
public class Benchmarks
{
    private readonly Parser _parser = new();
    private readonly ManualInterpolationEmitter _manualInterpolationEmitter = new();
    private InterchangingIncrementalGeneratorRunner _runner;

    private ImmutableArray<IMethodSymbol> _markedMethods;
    private IReadOnlyCollection<MappingMethod> _mappingMethods;

    [GlobalSetup]
    public void GlobalSetup()
    {
        var metadataReferences = new[]
        {
            MetadataReference.CreateFromFile(typeof(Binder).GetTypeInfo().Assembly.Location),
            MetadataReference.CreateFromFile(Assembly
                .Load("System.Runtime, Version=7.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a").Location),
            MetadataReference.CreateFromFile(Assembly.Load("netstandard, Version=2.0.0.0").Location),
            MetadataReference.CreateFromFile(typeof(GenerateMappingAttribute).GetTypeInfo().Assembly.Location)
        };

        var parameterMemorizingParser = new ParameterMemorizingParser(new Parser());
        var setupMappingGenerator = new MappingGenerator(
            parameterMemorizingParser,
            new ManualInterpolationEmitter());

        var setupRunner = new InterchangingIncrementalGeneratorRunner(
            setupMappingGenerator,
            SourceCodeConstants.AllMappingSource,
            metadataReferences);
        setupRunner.Invoke();
        _markedMethods = parameterMemorizingParser.MarkedMethods!.Value;
        _mappingMethods = parameterMemorizingParser.MappingMethods!;

        var defaultMappingGenerator = new MappingGenerator();
        
        _runner = new(
            defaultMappingGenerator,
            SourceCodeConstants.AllMappingSource,
            metadataReferences);
    }

    [Benchmark]
    public object GenerateFromScratch()
    {
        return _runner.Invoke();
    }

    [Benchmark]
    public object GenerateWithMemoization()
    {
        return _runner.InvokeWithMemoization();
    }

    [Benchmark]
    public object Parse()
    {
        return _parser.GetMapperClasses(_markedMethods, CancellationToken.None);
    }

    [Benchmark]
    public void ManualInterpolationEmit()
    {
        _manualInterpolationEmitter.Emit(
            AddSource,
            _mappingMethods,
            CancellationToken.None);
    }

    private static void AddSource(
        string hintName,
        string content)
    {
    }
}