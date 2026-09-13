using BlogDoFT.Libs.Flagr.Abstractions;
using Bogus;
using FlagrExample.Features.Impl;
using FlagrExample.Providers.Impl;

namespace FlagrExample.Tests.Providers;

public class FeatureProviderTests
{
    private static readonly Faker Faker = new();

    private static IFlagResolver CreateResolverReturning(string? variantKey)
    {
        var resolver = Substitute.For<IFlagResolver>();
        resolver
            .ResolveFlag<FeatureFlag>(Arg.Any<object>())
            .Returns(new EvaluationResponse { VariantKey = variantKey! });
        return resolver;
    }

    [Theory]
    [InlineData("Feature1", typeof(FeatureOne))]
    [InlineData("feature1", typeof(FeatureOne))]
    [InlineData("Feature2", typeof(FeatureTwo))]
    [InlineData("Unknow", typeof(FeatureUnknow))]
    [InlineData(null, typeof(FeatureUnknow))]
    [InlineData("", typeof(FeatureUnknow))]
    public void Should_ReturnMatchingFeature_When_ResolvedVariantKeyIsRecognized(string? variantKey, Type expectedFeatureType)
    {
        // Given
        var resolver = CreateResolverReturning(variantKey);
        var provider = new FeatureProvider(resolver, Faker.Company.CompanyName());

        // When
        var feature = provider.GetFeature();

        // Then
        feature.ShouldBeOfType(expectedFeatureType);
    }

    [Fact]
    public void Should_ThrowNullReferenceException_When_ResolvedVariantKeyIsNotARecognizedFlag()
    {
        // Given
        // Enum.TryParse(Type, string, bool, out object) leaves the out parameter as null
        // when the value can't be parsed, and FeatureProvider.ParseFlag casts it straight
        // to FeatureFlag without checking the TryParse result, so an unrecognized variant
        // key blows up instead of falling back to FeatureUnknow. Kept as a fixed literal
        // (rather than a Bogus-generated word) so it can never accidentally collide with
        // a real FeatureFlag member name.
        const string unrecognizedVariantKey = "totally-bogus";
        var resolver = CreateResolverReturning(unrecognizedVariantKey);
        var provider = new FeatureProvider(resolver, Faker.Company.CompanyName());

        // When
        var act = () => provider.GetFeature();

        // Then
        Should.Throw<NullReferenceException>(act);
    }

    [Fact]
    public void Should_PassApplicationNameInEntityContext_When_ResolvingFlag()
    {
        // Given
        var applicationName = Faker.Company.CompanyName();
        object? capturedContext = null;
        var resolver = Substitute.For<IFlagResolver>();
        resolver
            .ResolveFlag<FeatureFlag>(Arg.Do<object>(context => capturedContext = context))
            .Returns(new EvaluationResponse { VariantKey = "Feature1" });
        var provider = new FeatureProvider(resolver, applicationName);

        // When
        provider.GetFeature();

        // Then
        capturedContext.ShouldNotBeNull();
        var capturedApplicationName = capturedContext.GetType().GetProperty("ApplicationName")?.GetValue(capturedContext);
        capturedApplicationName.ShouldBe(applicationName);
    }
}
