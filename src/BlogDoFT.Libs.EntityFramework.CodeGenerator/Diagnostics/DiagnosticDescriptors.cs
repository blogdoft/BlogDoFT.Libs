using Microsoft.CodeAnalysis;
using System.Linq;

namespace BlogDoFT.Libs.EntityFramework.CodeGenerator.Diagnostics;

public static class DiagnosticDescriptors
{
    private const string DefaultCategoryUsage = "PredicateGenerator.Usage";
    private const string DefaultCategorySemantic = "PredicateGenerator.Semantics";
    private const string DefaultCategoryGeneration = "PredicateGenerator.Generation";
    private const string HelpBase = "https://example.com/docs/predicate-generator/"; // opcional: troque para sua URL

    public static readonly DiagnosticDescriptor FilterDtoMustBePartial = new(
        id: "PG001",
        title: "Filter DTO must be partial",
        messageFormat: "The DTO '{0}' must be declared as 'partial' to receive generated members",
        category: DefaultCategoryUsage,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description: "The source generator emits members (e.g., ToPredicate, HasFilter) into a partial type. Non-partial types cannot be augmented.",
        helpLinkUri: HelpLink("PG001"));

    public static readonly DiagnosticDescriptor TargetPropertyPathInvalid = new(
        id: "PG002",
        title: "Invalid TargetProperty path",
        messageFormat: "Property '{0}' on DTO '{1}' declares TargetProperty '{2}', which cannot be resolved on entity '{3}'",
        category: DefaultCategorySemantic,
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        description: "The TargetProperty must reference a valid member path on the target entity (supports dotted paths).",
        helpLinkUri: HelpLink("PG002"));

    public static readonly DiagnosticDescriptor UnsupportedFilterAttribute = new(
        id: "PG003",
        title: "Unsupported filter attribute",
        messageFormat: "Property '{0}' on DTO '{1}' is annotated with an unsupported attribute '{2}'. The attribute will be ignored.",
        category: DefaultCategorySemantic,
        defaultSeverity: DiagnosticSeverity.Info,
        isEnabledByDefault: true,
        description: "Only StringFilterAttribute, NumericFilterAttribute, TemporalFilterAttribute, and BooleanFilterAttribute are recognized.",
        helpLinkUri: HelpLink("PG003"));

    public static readonly DiagnosticDescriptor NoFilterablePropertiesFound = new(
        id: "PG004",
        title: "No filterable properties found",
        messageFormat: "DTO '{0}' has no filterable properties. The generated predicate will always return 'true'.",
        category: DefaultCategoryUsage,
        defaultSeverity: DiagnosticSeverity.Info,
        isEnabledByDefault: true,
        description: "The generator emits a trivial predicate when no annotated properties are present.",
        helpLinkUri: HelpLink("PG004"));

    public static readonly DiagnosticDescriptor ComparisonOperatorNotApplicable = new(
        id: "PG005",
        title: "Comparison operator not applicable",
        messageFormat: "Property '{0}' on DTO '{1}' declares a comparison operator that is not applicable to the target member '{2}'",
        category: DefaultCategorySemantic,
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        description: "Ensure the operator matches the target member type (numeric or temporal).",
        helpLinkUri: HelpLink("PG005"));

    public static readonly DiagnosticDescriptor EntityTypeNotResolved = new(
        id: "PG010",
        title: "Cannot resolve entity type",
        messageFormat: "Could not resolve the target entity type for DTO '{0}'. Ensure [GeneratePredicate<TEntity>] is correctly specified.",
        category: DefaultCategoryUsage,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description: "The generic attribute must provide a valid type argument corresponding to the target entity.",
        helpLinkUri: HelpLink("PG010"));

    public static readonly DiagnosticDescriptor GeneratedMembersSetToInternal = new(
        id: "PG101",
        title: "Generated members emitted as 'internal'",
        messageFormat: "The entity '{0}' or the DTO '{1}' is not public. Generated members were emitted as 'internal'.",
        category: DefaultCategoryGeneration,
        defaultSeverity: DiagnosticSeverity.Info,
        isEnabledByDefault: true,
        description: "ToPredicate and HasFilter visibility matches the least accessible type in their signatures.",
        helpLinkUri: HelpLink("PG101"));

    public static readonly DiagnosticDescriptor GenerationSucceeded = new(
        id: "PG200",
        title: "Predicate generation succeeded",
        messageFormat: "Generated predicate members for DTO '{0}'",
        category: DefaultCategoryGeneration,
        defaultSeverity: DiagnosticSeverity.Info,
        isEnabledByDefault: true,
        description: "Informational banner to confirm successful code emission.",
        helpLinkUri: HelpLink("PG200"));

    public static readonly DiagnosticDescriptor UnexpectedGenerationFailure = new(
        id: "PG900",
        title: "Unexpected error during code generation",
        messageFormat: "An unexpected error occurred while generating code for DTO '{0}': {1}",
        category: DefaultCategoryGeneration,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description: "Non-deterministic or unhandled exceptions surfaced during generation.",
        helpLinkUri: HelpLink("PG900"));

    private static string? HelpLink(string code) => HelpBase is { Length: > 0 } ? HelpBase + code.ToLowerInvariant() : null;

    public static Diagnostic Create(DiagnosticDescriptor descriptor, ISymbol? symbol, params object?[] messageArgs)
    {
        var location = symbol?.Locations.FirstOrDefault() ?? Location.None;
        return Diagnostic.Create(descriptor, location, messageArgs);
    }
}
