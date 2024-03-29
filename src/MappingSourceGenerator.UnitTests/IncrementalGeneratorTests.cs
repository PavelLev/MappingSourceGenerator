﻿using FluentAssertions;
using FluentAssertions.Execution;
using MappingSourceGenerator.UnitTests.Utility;
using Microsoft.CodeAnalysis.CSharp;

namespace MappingSourceGenerator.UnitTests;

public class IncrementalGeneratorTests
{
    [Theory]
    [MemberData(nameof(ManualMappingTestData.Data), MemberType = typeof(ManualMappingTestData))]
    [MemberData(nameof(NestedModelTestData.Data), MemberType = typeof(NestedModelTestData))]
    [MemberData(nameof(NonNestedClassTestData.Data), MemberType = typeof(NonNestedClassTestData))]
    [MemberData(nameof(SimpleCollectionTestData.Data), MemberType = typeof(SimpleCollectionTestData))]
    [MemberData(nameof(SimpleModelTestData.Data), MemberType = typeof(SimpleModelTestData))]
    public void SourceGeneratedOnlyOnceForTheSameCodeTest(
        string _0,
        string source,
        string _1)
    {
        var cSharpGeneratorDriver = CSharpGeneratorDriver.Create(new MappingGenerator());

        cSharpGeneratorDriver = cSharpGeneratorDriver.RunForCode(source);
        var firstRunResult = cSharpGeneratorDriver.GetRunResult();
        cSharpGeneratorDriver = cSharpGeneratorDriver.RunForCode(source);
        var secondRunResult = cSharpGeneratorDriver.GetRunResult();

        using (new AssertionScope())
        {
            var firstGeneratedSource = firstRunResult.ShouldGenerateSingleSource();
            var secondGeneratedSource = secondRunResult.ShouldGenerateSingleSource();
            ReferenceEquals(firstGeneratedSource, secondGeneratedSource).Should().BeTrue();
        }
    }
}