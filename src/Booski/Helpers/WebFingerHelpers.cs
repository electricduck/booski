using System.Net.Http.Json;
using Booski.Common;
using Booski.Contexts;

namespace Booski.Helpers;

public interface IWebFingerHelpers
{
    Task<WebFinger?> GetResource(string endpoint, string resource);
}

internal sealed class WebFingerHelpers : IWebFingerHelpers
{
    private IHttpContext _httpContext;

    public WebFingerHelpers(
        IHttpContext httpContext
    )
    {
        _httpContext = httpContext;
    }

    public async Task<WebFinger?> GetResource(string endpoint, string resource)
    {
#pragma warning disable CS8602 // Dereference of a possibly null reference.
        var httpResponse = await
            _httpContext.Client.GetAsync($"https://{endpoint}/.well-known/webfinger?resource={resource}");
#pragma warning restore CS8602 // Dereference of a possibly null reference.

        if (
            httpResponse != null &&
            httpResponse.IsSuccessStatusCode
        )
            return await httpResponse.Content.ReadFromJsonAsync<WebFinger>();

        return null;
    }
}
