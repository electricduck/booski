using Booski.Common;
using Mastonet;

namespace Booski.Contexts;

public interface IMastodonContext
{
    MastodonClient? Client { get; set; }
    bool IsConnected { get; set; }
    MastodonState? State { get; set; }

    Task CreateClient(string instance, string token);
    void ResetClient();
}

internal sealed class MastodonContext : IMastodonContext
{
    public MastodonClient? Client { get; set; }
    public bool IsConnected { get; set; }
    public MastodonState? State { get; set; }

    private IHttpContext _httpContext;

    public MastodonContext(
        IHttpContext httpContext
    )
    {
        _httpContext = httpContext;
    }

    public async Task CreateClient(
        string instance,
        string token
    )
    {
        if(_httpContext.Client == null)
            _httpContext.CreateClient();

        State = new MastodonState();

        var authClient = new AuthenticationClient(instance, _httpContext.Client);
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
        }
        else
        {
            ResetClient();
        }
    }

    public void ResetClient()
    {
        Client = null;
        IsConnected = false;
        State = null;
    }
}