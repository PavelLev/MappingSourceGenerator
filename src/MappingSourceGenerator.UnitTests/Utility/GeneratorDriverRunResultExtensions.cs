using System.Text;
using DiffPlex;
using DiffPlex.DiffBuilder;
using DiffPlex.DiffBuilder.Model;
using FluentAssertions;
using Microsoft.CodeAnalysis;

namespace MappingSourceGenerator.UnitTests.Utility;

public static class GeneratorDriverRunResultExtensions
{
    public static string ShouldGenerateSingleSource(
        this GeneratorDriverRunResult generatorDriverRunResult)
    {
        generatorDriverRunResult.Diagnostics.Should().BeEmpty();
        generatorDriverRunResult.Results.Length.Should().Be(1);
        var generatorRunResult = generatorDriverRunResult.Results.First();
        generatorRunResult.GeneratedSources.Length.Should().Be(1);
        var generatedSource = generatorRunResult.GeneratedSources.First();

        return generatedSource.SourceText.ToString();
    }
    
    public static void ShouldGenerate(
        this GeneratorDriverRunResult generatorDriverRunResult,
        string expectedGeneratedCode)
    {
        var generatedCode = generatorDriverRunResult.ShouldGenerateSingleSource();
        if (!generatedCode.Equals(expectedGeneratedCode, StringComparison.Ordinal))
        {
            var diff = GetDiff(expectedGeneratedCode, generatedCode);
            var message = $"Expected and actual source text of source differ: " + Environment.NewLine + diff;
            throw new InvalidOperationException(message);
        }
    }
    
    public static string GetDiff(string original, string modified)
    {
        var diffText = new StringBuilder();

        var differ = new Differ();
        var diffBuilder = new InlineDiffBuilder(differ);
        var diffModel = diffBuilder.BuildDiffModel(original, modified, false);

        foreach (var diffPiece in diffModel.Lines)
        {
            _ = diffPiece.Type switch
            {
                ChangeType.Inserted => diffText.Append('+'),
                ChangeType.Deleted => diffText.Append('-'),
                _ => diffText.Append(' '),
            };
            _ = diffText.AppendLine(diffPiece.Text);
        }

        return diffText.ToString();
    }
}