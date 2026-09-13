using BlogDoFT.Libs.Flagr;
using BlogDoFT.Libs.Flagr.Tests.TestSupport;
using Bogus;
using System.Net;
using System.Text;
using System.Text.Json;

namespace BlogDoFT.Libs.Flagr.Tests;

public class FlagResolverTests
{
    private static readonly Faker Faker = new();

    // Never instantiated: used only as a generic type argument to derive the flag key.
#pragma warning disable S2094
    private sealed class SampleEntity;
#pragma warning restore S2094

    private static (FlagResolver Resolver, FakeHttpMessageHandler Handler) CreateResolver(
        string responseBody,
        HttpStatusCode statusCode = HttpStatusCode.OK)
    {
        var handler = new FakeHttpMessageHandler(_ => new HttpResponseMessage(statusCode)
        {
            Content = new StringContent(responseBody, Encoding.UTF8, "application/json"),
        });
        var httpClient = new HttpClient(handler)
        {
            BaseAddress = new Uri("http://flagr.test/api/"),
        };
        return (new FlagResolver(httpClient), handler);
    }

    [Fact]
    public void Should_SendPostRequestToEvaluationEndpoint_When_ResolvingFlag()
    {
        // Given
        var (resolver, handler) = CreateResolver("{}");

        // When
        resolver.ResolveFlag<SampleEntity>(new { });

        // Then
        handler.LastRequest.ShouldNotBeNull();
        handler.LastRequest.Method.ShouldBe(HttpMethod.Post);
        handler.LastRequest.RequestUri!.ToString().ShouldBe("http://flagr.test/api/v1/evaluation");
    }

    [Fact]
    public void Should_SerializeRequestBodyUsingCamelCaseAndFlagKeyFromGenericType_When_ResolvingFlag()
    {
        // Given
        var applicationName = Faker.Company.CompanyName();
        var (resolver, handler) = CreateResolver("{}");

        // When
        resolver.ResolveFlag<SampleEntity>(new { ApplicationName = applicationName });

        // Then
        handler.LastRequestBody.ShouldNotBeNull();
        using var body = JsonDocument.Parse(handler.LastRequestBody);
        var root = body.RootElement;
        root.GetProperty("flagKey").GetString().ShouldBe(nameof(SampleEntity));
        root.GetProperty("entityContext").GetProperty("applicationName").GetString().ShouldBe(applicationName);
        root.TryGetProperty("FlagKey", out _).ShouldBeFalse();
    }

    [Fact]
    public void Should_DeserializeCamelCaseResponseBody_When_ResolvingFlag()
    {
        // Given
        var flagId = Faker.Random.Int(1, 1000);
        var flagSnapshotId = Faker.Random.Int(1, 1000);
        var segmentId = Faker.Random.Int(1, 1000);
        var variantId = Faker.Random.Int(1, 1000);
        var variantKey = Faker.Lorem.Word();
        var responseJson = $$"""
            {
                "flagId": {{flagId}},
                "flagKey": "{{nameof(SampleEntity)}}",
                "flagSnapshotID": {{flagSnapshotId}},
                "segmentID": {{segmentId}},
                "variantID": {{variantId}},
                "variantKey": "{{variantKey}}",
                "timestamp": "2026-01-01T00:00:00Z"
            }
            """;
        var (resolver, _) = CreateResolver(responseJson);

        // When
        var response = resolver.ResolveFlag<SampleEntity>(new { });

        // Then
        response.FlagId.ShouldBe(flagId);
        response.FlagKey.ShouldBe(nameof(SampleEntity));
        response.FlagSnapshotID.ShouldBe(flagSnapshotId);
        response.SegmentID.ShouldBe(segmentId);
        response.VariantID.ShouldBe(variantId);
        response.VariantKey.ShouldBe(variantKey);
    }

    [Fact]
    public void Should_NotThrow_When_ResponseStatusCodeIsNotSuccessful()
    {
        // Given
        var variantKey = Faker.Lorem.Word();
        var (resolver, _) = CreateResolver($$"""{"variantKey":"{{variantKey}}"}""", HttpStatusCode.NotFound);

        // When
        var response = resolver.ResolveFlag<SampleEntity>(new { });

        // Then
        response.VariantKey.ShouldBe(variantKey);
    }
}
