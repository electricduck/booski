using Booski.Common;
using LinqToTwitter;

namespace Booski.Contexts;

public interface IXContext
{
    TwitterContext? Client { get; set; }
    bool IsConnected { get; set; }
    XState? State { get; set; }

    Task CreateClient(string apiKey, string apiSecret, string accessToken, string accessSecret);
    void ResetClient();
}

internal sealed class XContext : IXContext
{
    public TwitterContext? Client { get; set; }
    public bool IsConnected { get; set; }
    public XState? State { get; set; }

    private IHttpContext _httpContext;

    public XContext(
        IHttpContext httpContext
    )
    {
        _httpContext = httpContext;
    }

    public async Task CreateClient(
        string apiKey,
        string apiSecret,
        string accessToken,
        string accessSecret
    )
    {
        if(_httpContext.Client == null)
            _httpContext.CreateClient();

        State = new XState();

        var authClient = new LinqToTwitter.OAuth.SingleUserAuthorizer
        {
            CredentialStore = new LinqToTwitter.OAuth.SingleUserInMemoryCredentialStore
            {
                ConsumerKey = apiKey,
                ConsumerSecret = apiSecret,
                AccessToken = accessToken,
                AccessTokenSecret = accessSecret
            }
        };
        Client = new TwitterContext(authClient);

        State.Account = await Client
            .Account    
            .Where(a => a.Type == AccountType.VerifyCredentials)
            .FirstOrDefaultAsync();

        if (State.Account != null)
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