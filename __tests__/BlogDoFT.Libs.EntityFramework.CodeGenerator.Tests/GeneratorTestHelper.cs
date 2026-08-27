using BlogDoFT.Libs.EntityFramework.CodeGenerator.Abstractions.PredicateGenerators;
using BlogDoFT.Libs.EntityFramework.CodeGenerator.Generators;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System.Collections.Immutable;
using System.Linq.Expressions;

namespace BlogDoFT.Libs.EntityFramework.CodeGenerator.Tests;

internal static class GeneratorTestHelper
{
    public static GeneratorRunResult Run(string source)
    {
        var syntaxTree = CSharpSyntaxTree.ParseText(source, new CSharpParseOptions(LanguageVersion.Latest));

        // Force the assemblies the generated/annotated source depends on to be loaded into the
        // current AppDomain before snapshotting it below (they may not be loaded yet otherwise,
        // since nothing in this test assembly's own IL references their types directly).
        _ = typeof(object).Assembly;
        _ = typeof(Enumerable).Assembly;
        _ = typeof(Expression).Assembly;
        _ = typeof(ComparisonOperator).Assembly;

        var references = AppDomain.CurrentDomain.GetAssemblies()
            .Where(assembly => !assembly.IsDynamic && !string.IsNullOrWhiteSpace(assembly.Location))
            .Select(assembly => (MetadataReference)MetadataReference.CreateFromFile(assembly.Location))
            .ToList();

        var compilation = CSharpCompilation.Create(
            assemblyName: "GeneratorTests",
            syntaxTrees: [syntaxTree],
            references: references,
            options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

        var generator = new PredicateGenerator();
        GeneratorDriver driver = CSharpGeneratorDriver.Create(generator);
        driver = driver.RunGeneratorsAndUpdateCompilation(compilation, out var outputCompilation, out _);

        var generatedTrees = outputCompilation.SyntaxTrees
            .Where(tree => tree.FilePath.EndsWith(".g.cs", StringComparison.Ordinal))
            .ToImmutableArray();

        // Diagnostics from actually compiling the output (source + generated trees together), so tests
        // can assert the emitted code is valid C# and not just that the generator didn't throw.
        var compilationErrors = outputCompilation.GetDiagnostics()
            .Where(diagnostic => diagnostic.Severity == DiagnosticSeverity.Error)
            .ToImmutableArray();

        return new GeneratorRunResult(driver.GetRunResult().Diagnostics, generatedTrees, compilationErrors);
    }
}

internal sealed class GeneratorRunResult
{
    public GeneratorRunResult(ImmutableArray<Diagnostic> diagnostics, ImmutableArray<SyntaxTree> generatedTrees, ImmutableArray<Diagnostic> compilationErrors)
    {
        Diagnostics = diagnostics;
        GeneratedTrees = generatedTrees;
        CompilationErrors = compilationErrors;
    }

    public ImmutableArray<Diagnostic> Diagnostics { get; }

    public ImmutableArray<SyntaxTree> GeneratedTrees { get; }

    /// <summary>Error-severity diagnostics from compiling the source together with the generated trees.</summary>
    public ImmutableArray<Diagnostic> CompilationErrors { get; }

    public string? GeneratedSourceEndingWith(string suffix) =>
        GeneratedTrees.FirstOrDefault(tree => tree.FilePath.EndsWith(suffix, StringComparison.Ordinal))?.ToString();

    public bool HasDiagnostic(string id) => Diagnostics.Any(diagnostic => diagnostic.Id == id);
}
