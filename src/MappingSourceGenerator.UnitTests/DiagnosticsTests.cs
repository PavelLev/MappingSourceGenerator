using FluentAssertions;
using FluentAssertions.Execution;
using MappingSourceGenerator.UnitTests.Utility;
using Microsoft.CodeAnalysis.CSharp;

namespace MappingSourceGenerator.UnitTests;

public class DiagnosticsTests
{
    [Theory]
    [MemberData(nameof(MappingMethodDiagnosticTestData.Data), MemberType = typeof(MappingMethodDiagnosticTestData))]
    [MemberData(nameof(MappingMethodParameterDiagnosticTestData.Data), MemberType = typeof(MappingMethodParameterDiagnosticTestData))]
    [MemberData(nameof(MappingMethodReturnTypeDiagnosticTestData.Data), MemberType = typeof(MappingMethodReturnTypeDiagnosticTestData))]
    [MemberData(nameof(InvalidMappingDiagnosticTestData.Data), MemberType = typeof(InvalidMappingDiagnosticTestData))]
    public void DiagnosticTest(string source, string diagnosticId, int line, int character)
    {
        var cSharpGeneratorDriver = CSharpGeneratorDriver.Create(new MappingGenerator());
        
        var generatorDriverRunResult = cSharpGeneratorDriver.RunForCode(source)
            .GetRunResult();

        using (new AssertionScope())
        {
            generatorDriverRunResult.Diagnostics.Length.Should().Be(1);
            var diagnostic = generatorDriverRunResult.Diagnostics[0];
            diagnostic.Descriptor.Id.Should().Be(diagnosticId);
            var lineSpan = diagnostic.Location.GetLineSpan();
            lineSpan.StartLinePosition.Line.Should().Be(line);
            lineSpan.StartLinePosition.Character.Should().Be(character);
        }
    }
}