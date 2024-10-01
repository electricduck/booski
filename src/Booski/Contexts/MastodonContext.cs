using Booski.Common;
using Booski.Helpers;
using Mastonet;
using Mastonet.Entities;

namespace Booski.Contexts;

public interface IMastodonContext
{
    MastodonClient? Client { get; set; }
    bool IsConnected { get; set; }
    MastodonState? State { get; set; }

    Task CreateClient(string instance, string token);
    Task CreateClient(string instance, string username, string password);
    void ResetClient();
}

internal sealed class MastodonContext : IMastodonContext
{
    public MastodonClient? Client { get; set; }
    public bool IsConnected { get; set; }
    public MastodonState? State { get; set; }

    private IHttpContext _httpContext;
    private IWebFingerHelpers _webFingerHelpers;

    public MastodonContext(
        IHttpContext httpContext,
        IWebFingerHelpers webFingerHelpers
    )
    {
        _httpContext = httpContext;
        _webFingerHelpers = webFingerHelpers;
    }

    public async Task CreateClient(
        string instance,
        string token
    )
    {
        if (_httpContext.Client == null)
            _httpContext.CreateClient();

        State = new MastodonState();

        try
        {
            Client = new MastodonClient(instance, token, _httpContext.Client);

            State.Instance = await Client.GetInstanceV2();
            State.Account = await Client.GetCurrentUser();

            if (
                State.Account != null &&
                State.Instance != null &&
                !String.IsNullOrEmpty(State.Account.Id)
            )
            {
                State.SetAdditionalFields();
                IsConnected = true;

                var webFingerRequest = await _webFingerHelpers.GetResource(State.Domain, State.Username);

                if(
                    webFingerRequest != null &&
                    webFingerRequest.Subject != null
                )
                    State.Username = webFingerRequest.Subject.Replace("acct:", "@");
            }
            else
                ResetClient();
        }
        catch
        {
            ResetClient();
        }
    }

    public async Task CreateClient(
        string instance,
        string username,
        string password
    )
    {
        var authClient = new AuthenticationClient(instance, _httpContext.Client);
        
        await authClient.CreateApp(
            "Booski",
            scope: new GranularScope[] {GranularScope.Read, GranularScope.Write},
            website: "https://github.com/electricduck/booski"
        );
        var auth = await authClient.ConnectWithPassword(username, password);

        await CreateClient(auth.AccessToken, instance);
    }

    public void ResetClient()
    {
        Client = null;
        IsConnected = false;
        State = null;
    }
}