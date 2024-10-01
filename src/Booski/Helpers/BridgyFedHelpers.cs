using System.Net.Http.Json;
using Booski.Common;
using Booski.Contexts;

namespace Booski.Helpers;

public interface IBridgyFedHelpers
{
    Task<string?> GetBridgyBskyHandle(string did);
    Task<WebFinger?> GetResource(string resource);
}

internal sealed class BridgyFedHelpers : IBridgyFedHelpers
{
    private IBskyHelpers _bskyHelpers;
    private IHttpContext _httpContext;
    private IWebFingerHelpers _webFingerHelpers;

    private string BridgyFedEndpoint { get; } = "https://fed.brid.gy";

    public BridgyFedHelpers(
        IBskyHelpers bskyHelpers,
        IHttpContext httpContext,
        IWebFingerHelpers webFingerHelpers
    )
    {
        _bskyHelpers = bskyHelpers;
        _httpContext = httpContext;
        _webFingerHelpers = webFingerHelpers;
    }

    public async Task<string?> GetBridgyBskyHandle(string did)
    {
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
        string bskyHandle = await _bskyHelpers.GetHandleForDid(did);
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
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
        return await _webFingerHelpers.GetResource(BridgyFedEndpoint, resource);
    }
}