#pragma warning disable S1192 // Define a literal for repeated string
using BlogDoFT.Libs.EntityFramework.CodeGenerator.Abstractions.PredicateGenerators;
using BlogDoFT.Libs.EntityFramework.CodeGenerator.Diagnostics;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BlogDoFT.Libs.EntityFramework.CodeGenerator.Generators
{
    /// <summary>
    /// Incremental source generator that emits <c>ToPredicate()</c> and <c>HasFilter()</c> members for
    /// partial DTOs decorated with <see cref="GeneratePredicateAttribute{TEntity}"/>, based on the
    /// <see cref="StringFilterAttribute"/>, <see cref="NumericFilterAttribute"/>, <see cref="TemporalFilterAttribute"/>,
    /// and <see cref="BooleanFilterAttribute"/> annotations declared on their properties.
    /// </summary>
    [Generator(LanguageNames.CSharp)]
    public sealed class PredicateGenerator : IIncrementalGenerator
    {
        /// <inheritdoc/>
        public void Initialize(IncrementalGeneratorInitializationContext context)
        {
            var candidates = context.SyntaxProvider
                .CreateSyntaxProvider(
                    static (syntaxNode, _) =>
                        syntaxNode is ClassDeclarationSyntax classDeclaration &&
                        classDeclaration.AttributeLists.Count > 0,
                    static (syntaxContext, cancellationToken) =>
                    {
                        var classDeclaration = (ClassDeclarationSyntax)syntaxContext.Node;
                        var filterTypeSymbol = syntaxContext.SemanticModel.GetDeclaredSymbol(classDeclaration, cancellationToken) as INamedTypeSymbol;
                        if (filterTypeSymbol is null)
                        {
                            return default(PredicateCandidate);
                        }

                        // Detect [GeneratePredicate<TEntity>] (generic attribute)
                        var generateAttribute = filterTypeSymbol.GetAttributes()
                            .FirstOrDefault(a =>
                                a.AttributeClass is { } attributeClass &&
                                attributeClass.Name.StartsWith("GeneratePredicateAttribute", StringComparison.Ordinal) &&
                                attributeClass.IsGenericType &&
                                attributeClass.TypeArguments.Length == 1);

                        if (generateAttribute is null)
                        {
                            return default(PredicateCandidate);
                        }

                        var entityTypeSymbol = generateAttribute.AttributeClass!.TypeArguments[0] as INamedTypeSymbol;
                        if (entityTypeSymbol is null || entityTypeSymbol.TypeKind == TypeKind.Error)
                        {
                            var unresolvedEntityDiagnostic = DiagnosticDescriptors.Create(
                                DiagnosticDescriptors.EntityTypeNotResolved,
                                filterTypeSymbol,
                                filterTypeSymbol.Name);

                            return new PredicateCandidate(
                                filterType: filterTypeSymbol,
                                entityType: null,
                                isPartial: false,
                                properties: Array.Empty<FilterProperty>(),
                                diagnostics: new[] { unresolvedEntityDiagnostic });
                        }

                        var isPartial = classDeclaration.Modifiers.Any(m => m.Text == "partial");

                        var propertySymbols = filterTypeSymbol.GetMembers()
                            .OfType<IPropertySymbol>()
                            .Where(p => p.DeclaredAccessibility == Accessibility.Public && p.GetMethod is not null)
                            .ToArray();

                        var collectedFilters = new List<FilterProperty>();
                        var propertyDiagnostics = new List<Diagnostic>();

                        foreach (var property in propertySymbols)
                        {
                            foreach (var attributeData in property.GetAttributes())
                            {
                                var attributeName = attributeData.AttributeClass?.Name;
                                if (attributeName is nameof(StringFilterAttribute) or nameof(NumericFilterAttribute) or nameof(TemporalFilterAttribute) or nameof(BooleanFilterAttribute))
                                {
                                    string? targetProperty = null;
                                    int? order = null;
                                    int? comparisonOperator = null;

                                    foreach (var namedArgument in attributeData.NamedArguments)
                                    {
                                        if (namedArgument.Key == "TargetProperty")
                                        {
                                            targetProperty = namedArgument.Value.Value as string;
                                        }
                                        else if (namedArgument.Key == "Order" && namedArgument.Value.Value is int orderValue)
                                        {
                                            order = orderValue;
                                        }
                                        else if (namedArgument.Key == "Operator" && namedArgument.Value.Value is int opValue)
                                        {
                                            comparisonOperator = opValue;
                                        }
                                    }

                                    var targetPropertyPath = targetProperty ?? property.Name;
                                    var resolvedTargetProperty = ResolveTargetProperty(entityTypeSymbol, targetPropertyPath);

                                    if (resolvedTargetProperty is null)
                                    {
                                        propertyDiagnostics.Add(DiagnosticDescriptors.Create(
                                            DiagnosticDescriptors.TargetPropertyPathInvalid,
                                            property,
                                            property.Name,
                                            filterTypeSymbol.Name,
                                            targetPropertyPath,
                                            entityTypeSymbol.Name));

                                        continue;
                                    }

                                    if (comparisonOperator is >= 1 and <= 4 && !SupportsOrdering(resolvedTargetProperty.Type))
                                    {
                                        propertyDiagnostics.Add(DiagnosticDescriptors.Create(
                                            DiagnosticDescriptors.ComparisonOperatorNotApplicable,
                                            property,
                                            property.Name,
                                            filterTypeSymbol.Name,
                                            targetPropertyPath));

                                        continue;
                                    }

                                    var declarationOrder = GetDeclarationOrder(property);

                                    collectedFilters.Add(new FilterProperty(
                                        propertySymbol: property,
                                        attributeName: attributeName!,
                                        targetProperty: targetProperty,
                                        order: order,
                                        comparisonOperator: comparisonOperator,
                                        declarationOrder: declarationOrder));
                                }
                                else if (attributeName is not null && attributeName.EndsWith("FilterAttribute", StringComparison.Ordinal))
                                {
                                    propertyDiagnostics.Add(DiagnosticDescriptors.Create(
                                        DiagnosticDescriptors.UnsupportedFilterAttribute,
                                        property,
                                        property.Name,
                                        filterTypeSymbol.Name,
                                        attributeName));
                                }
                            }
                        }

                        return new PredicateCandidate(
                            filterType: filterTypeSymbol,
                            entityType: entityTypeSymbol,
                            isPartial: isPartial,
                            properties: collectedFilters,
                            diagnostics: propertyDiagnostics);
                    })
                .Where(candidate => candidate.FilterType is not null);

            context.RegisterSourceOutput(candidates, static (productionContext, candidate) =>
            {
                try
                {
                    foreach (var diagnostic in candidate.Diagnostics)
                    {
                        productionContext.ReportDiagnostic(diagnostic);
                    }

                    if (candidate.EntityType is null)
                    {
                        return;
                    }

                    if (!candidate.IsPartial)
                    {
                        var diagnostic = DiagnosticDescriptors.Create(
                            DiagnosticDescriptors.FilterDtoMustBePartial,
                            candidate.FilterType,
                            candidate.FilterType!.Name);

                        productionContext.ReportDiagnostic(diagnostic);

                        return;
                    }

                    var result = GenerateSource(candidate);

                    if (!result.HasFilterableProperties)
                    {
                        productionContext.ReportDiagnostic(DiagnosticDescriptors.Create(
                            DiagnosticDescriptors.NoFilterablePropertiesFound,
                            candidate.FilterType,
                            candidate.FilterType!.Name));
                    }

                    if (result.IsInternal)
                    {
                        productionContext.ReportDiagnostic(DiagnosticDescriptors.Create(
                            DiagnosticDescriptors.GeneratedMembersSetToInternal,
                            candidate.FilterType,
                            candidate.EntityType!.Name,
                            candidate.FilterType!.Name));
                    }

                    productionContext.AddSource($"{candidate.FilterType!.Name}_ToPredicate.g.cs", result.Source);
                }
                catch (Exception exception)
                {
                    productionContext.ReportDiagnostic(DiagnosticDescriptors.Create(
                        DiagnosticDescriptors.UnexpectedGenerationFailure,
                        candidate.FilterType,
                        candidate.FilterType?.Name ?? "unknown",
                        exception.Message));
                }
            });
        }

        private static int GetDeclarationOrder(IPropertySymbol propertySymbol)
        {
            var syntaxReference = propertySymbol.DeclaringSyntaxReferences.FirstOrDefault();
            if (syntaxReference is null) return int.MaxValue;
            var location = syntaxReference.GetSyntax().GetLocation();
            var span = location.GetLineSpan();
            return span.StartLinePosition.Line * 1000 + span.StartLinePosition.Character;
        }

        private static IPropertySymbol? ResolveTargetProperty(INamedTypeSymbol entityType, string path)
        {
            ITypeSymbol currentType = entityType;
            IPropertySymbol? resolved = null;

            foreach (var segment in path.Split('.'))
            {
                resolved = FindPublicProperty(currentType, segment);
                if (resolved is null)
                {
                    return null;
                }

                currentType = resolved.Type;
            }

            return resolved;
        }

        private static IPropertySymbol? FindPublicProperty(ITypeSymbol type, string name)
        {
            for (var current = type; current is not null; current = current.BaseType)
            {
                var match = current.GetMembers(name)
                    .OfType<IPropertySymbol>()
                    .FirstOrDefault(p => p.DeclaredAccessibility == Accessibility.Public && p.GetMethod is not null);

                if (match is not null)
                {
                    return match;
                }
            }

            return null;
        }

        private static bool SupportsOrdering(ITypeSymbol type)
        {
            var underlying = UnwrapNullable(type);

            if (underlying.TypeKind == TypeKind.Enum)
            {
                return true;
            }

            switch (underlying.SpecialType)
            {
                case SpecialType.System_SByte:
                case SpecialType.System_Byte:
                case SpecialType.System_Int16:
                case SpecialType.System_UInt16:
                case SpecialType.System_Int32:
                case SpecialType.System_UInt32:
                case SpecialType.System_Int64:
                case SpecialType.System_UInt64:
                case SpecialType.System_Single:
                case SpecialType.System_Double:
                case SpecialType.System_Decimal:
                case SpecialType.System_Char:
                case SpecialType.System_DateTime:
                    return true;
            }

            if (underlying.ToDisplayString() is "System.DateTimeOffset" or "System.DateOnly" or "System.TimeOnly" or "System.TimeSpan")
            {
                return true;
            }

            // Types with a user-defined relational operator overload also work with Expression.GreaterThan/LessThan.
            return underlying.GetMembers("op_GreaterThan").OfType<IMethodSymbol>().Any();
        }

        private static ITypeSymbol UnwrapNullable(ITypeSymbol type) =>
            type is INamedTypeSymbol { OriginalDefinition.SpecialType: SpecialType.System_Nullable_T } nullableType
                ? nullableType.TypeArguments[0]
                : type;

        // Resolves the constant int stored on the attribute back to the real ComparisonOperator member
        // name, using the actual enum type (this project already references Abstractions), so the
        // generated switch binds to the enum by name instead of duplicating its numeric layout by hand.
        private static string ResolveComparisonOperatorName(int? comparisonOperator)
        {
            var value = comparisonOperator ?? (int)ComparisonOperator.Equal;

            return Enum.IsDefined(typeof(ComparisonOperator), value)
                ? ((ComparisonOperator)value).ToString()
                : nameof(ComparisonOperator.Equal);
        }

        private static GenerationResult GenerateSource(PredicateCandidate candidate)
        {
            var namespaceName = candidate.FilterType!.ContainingNamespace.IsGlobalNamespace
                ? null
                : candidate.FilterType.ContainingNamespace.ToDisplayString();

            var filterTypeName = candidate.FilterType.Name;
            var entityTypeName = candidate.EntityType!.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat);

            var methodAccessibility =
                candidate.FilterType.DeclaredAccessibility == Accessibility.Public &&
                candidate.EntityType.DeclaredAccessibility == Accessibility.Public
                ? "public" : "internal";

            var orderedFilters = candidate.Properties
                .OrderBy(p => p.Order ?? p.DeclarationOrder)
                .ToList();

            var builder = new StringBuilder();

            builder.AppendLine("// <auto-generated/>");
            builder.AppendLine("#nullable enable");
            if (namespaceName is not null) builder.AppendLine($"namespace {namespaceName};");
            builder.AppendLine("using System;");
            builder.AppendLine("using System.Linq;");
            builder.AppendLine("using System.Linq.Expressions;");
            builder.AppendLine("using BlogDoFT.Libs.EntityFramework.CodeGenerator.Abstractions.PredicateGenerators;");
            builder.AppendLine();

            builder.AppendLine($"partial class {filterTypeName}");
            builder.AppendLine("{");

            // HasFilter
            AddHasFilterMethod(builder, methodAccessibility, orderedFilters);

            // ToPredicate
            AddToPredicateMethod(builder, methodAccessibility, entityTypeName, orderedFilters);

            builder.AppendLine();
            builder.AppendLine("    private static MemberExpression BuildMemberExpression(ParameterExpression parameter, string path)");
            builder.AppendLine("    {");
            builder.AppendLine("        var segments = path.Split('.');");
            builder.AppendLine("        Expression current = parameter;");
            builder.AppendLine("        foreach (var segment in segments)");
            builder.AppendLine("        {");
            builder.AppendLine("            current = Expression.Property(current, segment);");
            builder.AppendLine("        }");
            builder.AppendLine("        return (MemberExpression)current;");
            builder.AppendLine("    }");

            builder.AppendLine();
            builder.AppendLine("    private static Expression BuildComparison(Expression left, Expression right, ComparisonOperator operatorValue) =>");
            builder.AppendLine("        operatorValue switch");
            builder.AppendLine("        {");
            builder.AppendLine("            ComparisonOperator.Equal => Expression.Equal(left, right),");
            builder.AppendLine("            ComparisonOperator.GreaterThan => Expression.GreaterThan(left, right),");
            builder.AppendLine("            ComparisonOperator.LessThan => Expression.LessThan(left, right),");
            builder.AppendLine("            ComparisonOperator.GreaterThanOrEqual => Expression.GreaterThanOrEqual(left, right),");
            builder.AppendLine("            ComparisonOperator.LessThanOrEqual => Expression.LessThanOrEqual(left, right),");
            builder.AppendLine("        };");

            builder.AppendLine("}");

            return new GenerationResult(
                source: builder.ToString(),
                hasFilterableProperties: orderedFilters.Count > 0,
                isInternal: methodAccessibility == "internal");
        }

        private static void AddHasFilterMethod(StringBuilder builder, string? methodAccessibility, List<FilterProperty> orderedFilters)
        {
            builder.AppendLine("    /// <summary>");
            builder.AppendLine("    /// Check if any query param was filled.");
            builder.AppendLine("    /// </summary>");
            builder.AppendLine("    /// <returns>True: The query has at least one property filled. False: None property has value.</returns>");
            builder.AppendLine($"    {methodAccessibility} bool HasFilter()");
            builder.AppendLine("    {");
            if (orderedFilters.Count == 0)
            {
                builder.AppendLine("        return false;");
            }
            else
            {
                var checks = string.Join(" || ", orderedFilters.Select(p => $"this.{p.PropertySymbol.Name} is not null"));
                builder.AppendLine($"        return {checks};");
            }
            builder.AppendLine("    }");
            builder.AppendLine();
        }

        private static void AddToPredicateMethod(StringBuilder builder, string? methodAccessibility, string entityTypeName, List<FilterProperty> orderedFilters)
        {
            // ToPredicate
            builder.AppendLine("    /// <summary>");
            builder.AppendLine("    /// Create a Predicate based on properties annotations rules.");
            builder.AppendLine("    /// </summary>");
            builder.AppendLine("    /// <remarks>The expression is compatible with EntityFramework SqlServer, Npgsql and Sqlite.</remarks>");
            builder.AppendLine("    /// <returns>A predicate to be used as a Linq expression.</returns>");
            builder.AppendLine($"    {methodAccessibility} Expression<Func<{entityTypeName}, bool>> ToPredicate()");
            builder.AppendLine("    {");
            builder.AppendLine($"        var entityParameter = Expression.Parameter(typeof({entityTypeName}), \"entity\");");
            builder.AppendLine("        Expression? aggregateBody = null;");
            builder.AppendLine();

            foreach (var filter in orderedFilters)
            {
                var sourcePropertyName = filter.PropertySymbol.Name;
                var targetPropertyPath = filter.TargetProperty ?? sourcePropertyName;

                builder.AppendLine($"        if (this.{sourcePropertyName} is not null)");
                builder.AppendLine("        {");
                builder.AppendLine($"            var targetMember = BuildMemberExpression(entityParameter, \"{targetPropertyPath}\");");

                if (filter.AttributeName == nameof(StringFilterAttribute))
                {
                    builder.AppendLine($"            var rawObject = (object)this.{sourcePropertyName}!;");
                    builder.AppendLine("            var rawText = rawObject.ToString();");
                    builder.AppendLine("            if (rawText is not null)");
                    builder.AppendLine("            {");
                    builder.AppendLine("                Expression comparison;");
                    builder.AppendLine("                if (!rawText.Contains(\"%\"))");
                    builder.AppendLine("                {");
                    builder.AppendLine("                    var right = Expression.Constant(rawText, typeof(string));");
                    builder.AppendLine("                    comparison = Expression.Equal(targetMember, right);");
                    builder.AppendLine("                }");
                    builder.AppendLine("                else");
                    builder.AppendLine("                {");
                    builder.AppendLine("                    var startsWithPercent = rawText.StartsWith(\"%\", StringComparison.Ordinal);");
                    builder.AppendLine("                    var endsWithPercent = rawText.EndsWith(\"%\", StringComparison.Ordinal);");
                    builder.AppendLine("                    var trimmed = rawText.Trim('%');");
                    builder.AppendLine("                    var cleaned = rawText.Replace(\"%\", string.Empty);");

                    builder.AppendLine("                    if (startsWithPercent && endsWithPercent)");
                    builder.AppendLine("                    {");
                    builder.AppendLine("                        var contains = typeof(string).GetMethod(nameof(string.Contains), new[] { typeof(string) })!;");
                    builder.AppendLine("                        comparison = Expression.Call(targetMember, contains, Expression.Constant(trimmed));");
                    builder.AppendLine("                    }");
                    builder.AppendLine("                    else if (startsWithPercent && !endsWithPercent)");
                    builder.AppendLine("                    {");
                    builder.AppendLine("                        var endsWith = typeof(string).GetMethod(nameof(string.EndsWith), new[] { typeof(string) })!;");
                    builder.AppendLine("                        comparison = Expression.Call(targetMember, endsWith, Expression.Constant(trimmed));");
                    builder.AppendLine("                    }");
                    builder.AppendLine("                    else if (!startsWithPercent && endsWithPercent)");
                    builder.AppendLine("                    {");
                    builder.AppendLine("                        var startsWith = typeof(string).GetMethod(nameof(string.StartsWith), new[] { typeof(string) })!;");
                    builder.AppendLine("                        comparison = Expression.Call(targetMember, startsWith, Expression.Constant(trimmed));");
                    builder.AppendLine("                    }");
                    builder.AppendLine("                    else");
                    builder.AppendLine("                    {");
                    builder.AppendLine("                        var contains = typeof(string).GetMethod(nameof(string.Contains), new[] { typeof(string) })!;");
                    builder.AppendLine("                        comparison = Expression.Call(targetMember, contains, Expression.Constant(cleaned));");
                    builder.AppendLine("                    }");
                    builder.AppendLine("                }");
                    builder.AppendLine("                aggregateBody = aggregateBody is null ? comparison : Expression.AndAlso(aggregateBody, comparison);");
                    builder.AppendLine("            }");
                }
                else if (filter.AttributeName == nameof(NumericFilterAttribute) || filter.AttributeName == nameof(TemporalFilterAttribute))
                {
                    builder.AppendLine($"            var right = Expression.Convert(Expression.Constant(this.{sourcePropertyName}!), targetMember.Type);");
                    var operatorMemberName = ResolveComparisonOperatorName(filter.ComparisonOperator);
                    builder.AppendLine($"            var comparison = BuildComparison(targetMember, right, ComparisonOperator.{operatorMemberName});");
                    builder.AppendLine("            aggregateBody = aggregateBody is null ? comparison : Expression.AndAlso(aggregateBody, comparison);");
                }
                else if (filter.AttributeName == nameof(BooleanFilterAttribute))
                {
                    builder.AppendLine($"            var right = Expression.Convert(Expression.Constant(this.{sourcePropertyName}!), targetMember.Type);");
                    builder.AppendLine("            var comparison = Expression.Equal(targetMember, right);");
                    builder.AppendLine("            aggregateBody = aggregateBody is null ? comparison : Expression.AndAlso(aggregateBody, comparison);");
                }

                builder.AppendLine("        }");
                builder.AppendLine();
            }

            builder.AppendLine("        if (aggregateBody is null)");
            builder.AppendLine("        {");
            builder.AppendLine("            return entity => true;");
            builder.AppendLine("        }");
            builder.AppendLine("        return Expression.Lambda<Func<" + entityTypeName + ", bool>>(aggregateBody, entityParameter);");
            builder.AppendLine("}");
        }

        private readonly struct GenerationResult
        {
            public string Source { get; }
            public bool HasFilterableProperties { get; }
            public bool IsInternal { get; }

            public GenerationResult(string source, bool hasFilterableProperties, bool isInternal)
            {
                Source = source;
                HasFilterableProperties = hasFilterableProperties;
                IsInternal = isInternal;
            }
        }

        private readonly struct PredicateCandidate
        {
            public INamedTypeSymbol? FilterType { get; }
            public INamedTypeSymbol? EntityType { get; }
            public bool IsPartial { get; }
            public IReadOnlyList<FilterProperty> Properties { get; }
            public IReadOnlyList<Diagnostic> Diagnostics { get; }

            public PredicateCandidate(
                INamedTypeSymbol? filterType,
                INamedTypeSymbol? entityType,
                bool isPartial,
                IReadOnlyList<FilterProperty> properties,
                IReadOnlyList<Diagnostic> diagnostics)
            {
                FilterType = filterType;
                EntityType = entityType;
                IsPartial = isPartial;
                Properties = properties;
                Diagnostics = diagnostics;
            }
        }

        private readonly struct FilterProperty
        {
            public IPropertySymbol PropertySymbol { get; }
            public string AttributeName { get; }
            public string? TargetProperty { get; }
            public int? Order { get; }
            public int? ComparisonOperator { get; }
            public int DeclarationOrder { get; }

            public FilterProperty(IPropertySymbol propertySymbol, string attributeName, string? targetProperty, int? order, int? comparisonOperator, int declarationOrder)
            {
                PropertySymbol = propertySymbol;
                AttributeName = attributeName;
                TargetProperty = targetProperty;
                Order = order;
                ComparisonOperator = comparisonOperator;
                DeclarationOrder = declarationOrder;
            }
        }
    }
}
