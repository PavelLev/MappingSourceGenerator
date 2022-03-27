using System.Collections.Immutable;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace MappingSourceGenerator;

[Generator]
public class MappingGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        // System.Diagnostics.Debugger.Launch();

        IncrementalValuesProvider<ClassDeclarationSyntax> classDeclarations = context.SyntaxProvider
            .CreateSyntaxProvider((s, _) => Parser.IsSyntaxTargetForGeneration(s), (ctx, _) => Parser.GetSemanticTargetForGeneration(ctx))
            .Where(m => m is not null)!;

        var compilationAndClasses =
            context.CompilationProvider.Combine(classDeclarations.Collect());

        context.RegisterSourceOutput(compilationAndClasses, static (spc, source) => Execute(source.Item1, source.Item2, spc));
    }

    private static void Execute(Compilation compilation, ImmutableArray<ClassDeclarationSyntax> classes, SourceProductionContext context)
    {
        if (classes.IsDefaultOrEmpty)
        {
            // nothing to do yet
            return;
        }

        var distinctClasses = classes.Distinct();

        var parser = new Parser(compilation, context.ReportDiagnostic, context.CancellationToken);
        var mappingClasses = parser.GetMapperClasses(distinctClasses);

        if (mappingClasses.Count > 0)
        {
            var e = new Emitter();
            foreach (var mappingClass in mappingClasses)
            {
                context.CancellationToken.ThrowIfCancellationRequested();
                var (baseFileName, content) = e.Emit(mappingClass);

                context.AddSource($"{baseFileName}.g.cs", SourceText.From(content, Encoding.UTF8));
            }

        }
    }
}