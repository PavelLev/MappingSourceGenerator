using System.Reflection;
using MappingSourceGenerator.Markers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace MappingSourceGenerator.UnitTests.Utility;

public static class GeneratorRunnerExtensions
{
    public const string GeneratedAssemblyName = "CompilerGeneratedCompilation";

    private static IReadOnlyCollection<MetadataReference> DefaultMetadataReferences = new[]
    {
        MetadataReference.CreateFromFile(
            typeof(Binder).GetTypeInfo().Assembly.Location),
        MetadataReference.CreateFromFile(
            Assembly.Load("System.Runtime, Version=7.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a").Location),
        MetadataReference.CreateFromFile(
            Assembly.Load("netstandard, Version=2.0.0.0").Location),
        MetadataReference.CreateFromFile(
            typeof(GenerateMappingAttribute).GetTypeInfo().Assembly.Location)
    };

    public static TGeneratorDriver RunForCode<TGeneratorDriver>(
        this TGeneratorDriver generatorDriver,
        string source,
        IEnumerable<MetadataReference>? metadataReferences = null)
        where TGeneratorDriver : GeneratorDriver
    {
        var compilation = CSharpCompilation.Create(
            GeneratedAssemblyName,
            new[] { CSharpSyntaxTree.ParseText(source) },
            metadataReferences ?? DefaultMetadataReferences);

        return (TGeneratorDriver)generatorDriver
            .RunGenerators(compilation);
    }
}