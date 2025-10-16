using MappingSourceGenerator.UnitTests.Utility;
using Microsoft.CodeAnalysis.CSharp;

namespace MappingSourceGenerator.UnitTests;

public class GenerationResultTests
{
    [Theory]
    [MemberData(nameof(ManualMappingTestData.Data), MemberType = typeof(ManualMappingTestData))]
    [MemberData(nameof(NestedModelTestData.Data), MemberType = typeof(NestedModelTestData))]
    [MemberData(nameof(SimpleCollectionTestData.Data), MemberType = typeof(SimpleCollectionTestData))]
    [MemberData(nameof(SimpleModelTestData.Data), MemberType = typeof(SimpleModelTestData))]
    [MemberData(nameof(UsableMapMethodNamesTestData.Data), MemberType = typeof(UsableMapMethodNamesTestData))]
    public void GeneratedResultTest(string _, string source, string expectedGeneratedResult)
    {
        var cSharpGeneratorDriver = CSharpGeneratorDriver.Create(new MappingGenerator());

        var generatorDriverRunResult = cSharpGeneratorDriver.RunForCode(source)
            .GetRunResult();
        
        generatorDriverRunResult.ShouldGenerate(expectedGeneratedResult);
    }
}