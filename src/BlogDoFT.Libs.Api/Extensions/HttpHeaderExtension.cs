using Microsoft.AspNetCore.Http;
using System.Diagnostics.CodeAnalysis;

namespace BlogDoFT.Libs.Api.Extensions;

/// <summary>
/// Provides extension methods for reading well-known values from HTTP request headers.
/// </summary>
[ExcludeFromCodeCoverage]
public static class HttpHeaderExtension
{
    private const string CorrelationId = "X-Correlation-ID";
    private const string ForwardedHost = "X-Forwarded-Host";
    private const string AcceptLanguageHeader = "Accept-Language";

    /// <summary>
    /// Gets the correlation id from the <c>X-Correlation-ID</c> header, generating a new one if it is missing or empty.
    /// </summary>
    /// <param name="headers">The header collection to read from.</param>
    /// <returns>The correlation id found in the headers, or a newly generated <see cref="Guid"/> as a string when none is present.</returns>
    public static string GetCorrelationId(this IHeaderDictionary headers)
    {
        var hasCorrelationId = headers.TryGetValue(CorrelationId, out var headerValue);
        var correlationId = headerValue.ToString();
        if (!hasCorrelationId || string.IsNullOrWhiteSpace(correlationId))
        {
            correlationId = Guid.NewGuid().ToString();
        }

        return correlationId;
    }

    /// <summary>
    /// Gets the forwarded host from the <c>X-Forwarded-Host</c> header, falling back to the request's own host when the header is not set.
    /// </summary>
    /// <param name="httpRequest">The request to read the host from.</param>
    /// <returns>The forwarded host value, or the request's host if the header is missing or empty.</returns>
    public static string GetForwardedHost(this HttpRequest httpRequest)
    {
        var host = httpRequest.Headers[ForwardedHost];
        if (string.IsNullOrWhiteSpace(host))
        {
            host = httpRequest.Host.Value;
        }

        return host!;
    }

    /// <summary>
    /// Gets the first language listed in the <c>Accept-Language</c> header, falling back to <paramref name="defaultLanguage"/> when the header is missing or empty.
    /// </summary>
    /// <param name="headers">The header collection to read from.</param>
    /// <param name="defaultLanguage">The value to return when the <c>Accept-Language</c> header is not present. Defaults to an empty string.</param>
    /// <returns>The first language from the <c>Accept-Language</c> header, or <paramref name="defaultLanguage"/> when none is present.</returns>
    public static string GetDefaultAcceptLanguage(
        this IHeaderDictionary headers,
        string defaultLanguage = "")
    {
        var acceptHeader = headers[AcceptLanguageHeader].ToString()
            .Split(',')
            .FirstOrDefault();

        if (string.IsNullOrWhiteSpace(acceptHeader))
        {
            acceptHeader = defaultLanguage;
        }

        return acceptHeader;
    }
}
