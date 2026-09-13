using BlogDoFT.Libs.Flagr.Abstractions;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace BlogDoFT.Libs.Flagr;

public class FlagEvaluator : IFlagEvaluator
{
    private static readonly JsonSerializerOptions JsonOptions = new JsonSerializerOptions
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
    };

    private readonly HttpClient _httpClient;

    public FlagEvaluator(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public EvaluationResponse EvaluateFlag<T>(object entityContext)
    {
        var request = GetRequest(typeof(T), entityContext);
        return GetFlagAsync(request).Result;
    }

    private EvaluationRequest GetRequest(Type type, object entityContext) =>
        new EvaluationRequest
        {
            FlagKey = type.Name,
            EntityContext = entityContext,
        };

    private async Task<EvaluationResponse> GetFlagAsync(EvaluationRequest request)
    {
        using var response = await _httpClient
            .PostAsJsonAsync("v1/evaluation", request, JsonOptions)
            .ConfigureAwait(false);

        return await response.Content
            .ReadFromJsonAsync<EvaluationResponse>(JsonOptions)
            .ConfigureAwait(false);
    }
}
