using Shouldly;

namespace BlogDoFT.Libs.EntityFramework.CodeGenerator.Tests;

public class PredicateGeneratorTests
{
    [Fact]
    public void GivenDtoWithFilterableProperties_WhenGenerating_ThenEmitsPublicPredicateWithoutWarnings()
    {
        const string Source = """
            using BlogDoFT.Libs.EntityFramework.CodeGenerator.Abstractions.PredicateGenerators;

            namespace TestNamespace;

            public class Entity
            {
                public string? Name { get; set; }
            }

            [GeneratePredicate<Entity>]
            public partial class EntityFilter
            {
                [StringFilter(TargetProperty = nameof(Entity.Name))]
                public string? Name { get; set; }
            }
            """;

        var result = GeneratorTestHelper.Run(Source);

        result.HasDiagnostic("PG004").ShouldBeFalse();
        result.HasDiagnostic("PG101").ShouldBeFalse();
        result.HasDiagnostic("PG900").ShouldBeFalse();
        result.CompilationErrors.ShouldBeEmpty();

        var generated = result.GeneratedSourceEndingWith("EntityFilter_ToPredicate.g.cs");
        generated.ShouldNotBeNull();
        generated.ShouldContain("public Expression<Func<Entity, bool>> ToPredicate()");
    }

    [Fact]
    public void GivenNumericFilterWithOrderingOperator_WhenGenerating_ThenEmitsCompilableNamedOperatorSwitch()
    {
        const string Source = """
            using BlogDoFT.Libs.EntityFramework.CodeGenerator.Abstractions.PredicateGenerators;

            namespace TestNamespace;

            public class Entity
            {
                public int Age { get; set; }
            }

            [GeneratePredicate<Entity>]
            public partial class EntityFilter
            {
                [NumericFilter(TargetProperty = nameof(Entity.Age), Operator = ComparisonOperator.GreaterThanOrEqual)]
                public int? MinimumAge { get; set; }
            }
            """;

        var result = GeneratorTestHelper.Run(Source);

        result.HasDiagnostic("PG005").ShouldBeFalse();
        result.CompilationErrors.ShouldBeEmpty();

        var generated = result.GeneratedSourceEndingWith("EntityFilter_ToPredicate.g.cs");
        generated.ShouldNotBeNull();
        generated.ShouldContain("using BlogDoFT.Libs.EntityFramework.CodeGenerator.Abstractions.PredicateGenerators;");
        generated.ShouldContain("ComparisonOperator operatorValue) =>");
        generated.ShouldContain("BuildComparison(targetMember, right, ComparisonOperator.GreaterThanOrEqual);");
        generated.ShouldContain("ComparisonOperator.GreaterThan => Expression.GreaterThan(left, right),");
    }

    [Fact]
    public void GivenDtoWithNoFilterableProperties_WhenGenerating_ThenReportsPG004()
    {
        const string Source = """
            using BlogDoFT.Libs.EntityFramework.CodeGenerator.Abstractions.PredicateGenerators;

            namespace TestNamespace;

            public class Entity
            {
                public string? Name { get; set; }
            }

            [GeneratePredicate<Entity>]
            public partial class EmptyFilter
            {
                public string? NotAFilter { get; set; }
            }
            """;

        var result = GeneratorTestHelper.Run(Source);

        result.HasDiagnostic("PG004").ShouldBeTrue();
        result.CompilationErrors.ShouldBeEmpty();

        var generated = result.GeneratedSourceEndingWith("EmptyFilter_ToPredicate.g.cs");
        generated.ShouldNotBeNull();
        generated.ShouldContain("return false;");
        generated.ShouldContain("return entity => true;");
    }

    [Fact]
    public void GivenInternalEntity_WhenGenerating_ThenReportsPG101AndEmitsInternalMembers()
    {
        const string Source = """
            using BlogDoFT.Libs.EntityFramework.CodeGenerator.Abstractions.PredicateGenerators;

            namespace TestNamespace;

            internal class Entity
            {
                public string? Name { get; set; }
            }

            [GeneratePredicate<Entity>]
            public partial class EntityFilter
            {
                [StringFilter(TargetProperty = nameof(Entity.Name))]
                public string? Name { get; set; }
            }
            """;

        var result = GeneratorTestHelper.Run(Source);

        result.HasDiagnostic("PG101").ShouldBeTrue();
        result.CompilationErrors.ShouldBeEmpty();

        var generated = result.GeneratedSourceEndingWith("EntityFilter_ToPredicate.g.cs");
        generated.ShouldNotBeNull();
        generated.ShouldContain("internal Expression<Func<Entity, bool>> ToPredicate()");
    }

    [Fact]
    public void GivenNonPartialDto_WhenGenerating_ThenReportsPG001AndEmitsNoSource()
    {
        const string Source = """
            using BlogDoFT.Libs.EntityFramework.CodeGenerator.Abstractions.PredicateGenerators;

            namespace TestNamespace;

            public class Entity
            {
                public string? Name { get; set; }
            }

            [GeneratePredicate<Entity>]
            public class NotPartialFilter
            {
                [StringFilter(TargetProperty = nameof(Entity.Name))]
                public string? Name { get; set; }
            }
            """;

        var result = GeneratorTestHelper.Run(Source);

        result.HasDiagnostic("PG001").ShouldBeTrue();
        result.GeneratedSourceEndingWith("NotPartialFilter_ToPredicate.g.cs").ShouldBeNull();
        result.CompilationErrors.ShouldBeEmpty();
    }

    [Fact]
    public void GivenClassWithoutGeneratePredicateAttribute_WhenGenerating_ThenProducesNoOutput()
    {
        const string Source = """
            namespace TestNamespace;

            public partial class PlainClass
            {
                public string? Name { get; set; }
            }
            """;

        var result = GeneratorTestHelper.Run(Source);

        result.Diagnostics.ShouldBeEmpty();
        result.GeneratedTrees.ShouldBeEmpty();
        result.CompilationErrors.ShouldBeEmpty();
    }

    [Fact]
    public void GivenUnresolvableEntityType_WhenGenerating_ThenReportsPG010AndEmitsNoSource()
    {
        const string Source = """
            using BlogDoFT.Libs.EntityFramework.CodeGenerator.Abstractions.PredicateGenerators;

            namespace TestNamespace;

            [GeneratePredicate<DoesNotExist>]
            public partial class EntityFilter
            {
                public string? Name { get; set; }
            }
            """;

        var result = GeneratorTestHelper.Run(Source);

        result.HasDiagnostic("PG010").ShouldBeTrue();
        result.GeneratedSourceEndingWith("EntityFilter_ToPredicate.g.cs").ShouldBeNull();
    }

    [Fact]
    public void GivenInvalidTargetPropertyPath_WhenGenerating_ThenReportsPG002AndSkipsThatFilter()
    {
        const string Source = """
            using BlogDoFT.Libs.EntityFramework.CodeGenerator.Abstractions.PredicateGenerators;

            namespace TestNamespace;

            public class Entity
            {
                public string? Name { get; set; }
            }

            [GeneratePredicate<Entity>]
            public partial class EntityFilter
            {
                [StringFilter(TargetProperty = "DoesNotExist")]
                public string? Name { get; set; }
            }
            """;

        var result = GeneratorTestHelper.Run(Source);

        result.HasDiagnostic("PG002").ShouldBeTrue();
        result.CompilationErrors.ShouldBeEmpty();

        var generated = result.GeneratedSourceEndingWith("EntityFilter_ToPredicate.g.cs");
        generated.ShouldNotBeNull();
        generated.ShouldContain("return entity => true;");
    }

    [Fact]
    public void GivenUnsupportedFilterAttribute_WhenGenerating_ThenReportsPG003AndIgnoresIt()
    {
        const string Source = """
            using BlogDoFT.Libs.EntityFramework.CodeGenerator.Abstractions.PredicateGenerators;

            namespace TestNamespace;

            public sealed class GuidFilterAttribute : System.Attribute
            {
                public string? TargetProperty { get; set; }
            }

            public class Entity
            {
                public System.Guid Id { get; set; }
            }

            [GeneratePredicate<Entity>]
            public partial class EntityFilter
            {
                [GuidFilter(TargetProperty = nameof(Entity.Id))]
                public System.Guid? Id { get; set; }
            }
            """;

        var result = GeneratorTestHelper.Run(Source);

        result.HasDiagnostic("PG003").ShouldBeTrue();
        result.CompilationErrors.ShouldBeEmpty();

        var generated = result.GeneratedSourceEndingWith("EntityFilter_ToPredicate.g.cs");
        generated.ShouldNotBeNull();
        generated.ShouldContain("return entity => true;");
    }

    [Fact]
    public void GivenOrderingOperatorOnNonOrderableTargetMember_WhenGenerating_ThenReportsPG005AndSkipsThatFilter()
    {
        const string Source = """
            using BlogDoFT.Libs.EntityFramework.CodeGenerator.Abstractions.PredicateGenerators;

            namespace TestNamespace;

            public class Entity
            {
                public string? Name { get; set; }
            }

            [GeneratePredicate<Entity>]
            public partial class EntityFilter
            {
                [NumericFilter(TargetProperty = nameof(Entity.Name), Operator = ComparisonOperator.GreaterThan)]
                public int? NameFilter { get; set; }
            }
            """;

        var result = GeneratorTestHelper.Run(Source);

        result.HasDiagnostic("PG005").ShouldBeTrue();
        result.CompilationErrors.ShouldBeEmpty();

        var generated = result.GeneratedSourceEndingWith("EntityFilter_ToPredicate.g.cs");
        generated.ShouldNotBeNull();
        generated.ShouldContain("return entity => true;");
    }
}
