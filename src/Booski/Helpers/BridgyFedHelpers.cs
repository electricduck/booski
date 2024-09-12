using System.Net.Http.Json;
using Booski.Common;
using Booski.Contexts;

namespace Booski.Helpers;

public interface IBridgyFedHelpers
{
    Task<string?> GetBridgyBskyHandle(string did);
    Task<WebFinger> GetResource(string resource);
}

internal sealed class BridgyFedHelpers : IBridgyFedHelpers
{
    private IBskyHelpers _bskyHelpers;
    private IHttpContext _httpContext;

    private string BridgyFedEndpoint { get; } = "https://fed.brid.gy";

    public BridgyFedHelpers(
        IBskyHelpers bskyHelpers,
        IHttpContext httpContext
    )
    {
        _bskyHelpers = bskyHelpers;
        _httpContext = httpContext;
    }

    public async Task<string?> GetBridgyBskyHandle(string did)
    {
        string bskyHandle = await _bskyHelpers.GetHandleForDid(did);
        string bskyBridgyHandle = null;

        if(bskyHandle != null)
        {
            var webFingerRequest = await GetResource($"acct:{bskyHandle}@bsky.brid.gy");
            if(webFingerRequest != null)
                bskyBridgyHandle = webFingerRequest.Subject.Replace("acct:", "");
        }

        return bskyBridgyHandle;
    }

    public async Task<WebFinger?> GetResource(string resource)
    {
        var httpResponse = await
            _httpContext.Client.GetAsync($"{BridgyFedEndpoint}/.well-known/webfinger?resource={resource}");
        
        if(
            httpResponse != null &&
            httpResponse.IsSuccessStatusCode
        )
            return await httpResponse.Content.ReadFromJsonAsync<WebFinger>();

        return null;
    }
}