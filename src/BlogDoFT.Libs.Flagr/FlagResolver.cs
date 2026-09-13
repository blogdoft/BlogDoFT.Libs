using BlogDoFT.Libs.Flagr.Abstractions;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace BlogDoFT.Libs.Flagr;

public class FlagResolver : IFlagResolver
{
    private static readonly JsonSerializerOptions JsonOptions = new JsonSerializerOptions
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
    };

    private readonly HttpClient _httpClient;

    public FlagResolver(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public EvaluationResponse ResolveFlag<T>(object entityContext)
    {
        var request = GetRequest(typeof(T), entityContext);
        return GetFlag(request).Result;
    }

    private EvaluationRequest GetRequest(Type type, object entityContext) =>
        new EvaluationRequest
        {
            FlagKey = type.Name,
            EntityContext = entityContext,
        };

    private async Task<EvaluationResponse> GetFlag(EvaluationRequest request)
    {
        using var response = await _httpClient
            .PostAsJsonAsync("v1/evaluation", request, JsonOptions)
            .ConfigureAwait(false);

        return await response.Content
            .ReadFromJsonAsync<EvaluationResponse>(JsonOptions)
            .ConfigureAwait(false);
    }
}
