using Booski.Common;
using Telegram.Bot;

namespace Booski.Contexts;

public interface ITelegramContext
{
    TelegramBotClient? Client { get; set; }
    bool IsConnected { get; set; }
    TelegramState? State { get; set; }

    Task CreateClient(string token, string channel = "");
    void ResetClient();
}

internal sealed class TelegramContext : ITelegramContext
{
    public TelegramBotClient? Client { get; set; }
    public bool IsConnected { get; set; }
    public TelegramState? State { get; set; }

    private IHttpContext _httpContext;

    public TelegramContext(
        IHttpContext httpContext
    )
    {
        _httpContext = httpContext;
    }

    public async Task CreateClient(
        string token,
        string channel = ""
    )
    {
        if(_httpContext.Client == null)
            _httpContext.CreateClient();

        State = new TelegramState();
        Client = new TelegramBotClient(token, _httpContext.Client);

        var clientTestApi = await Client.TestApiAsync();

        if(clientTestApi)
        {
            State.Account = await Client.GetMeAsync();
            State.Channel = channel; // TODO: Check if channel is valid?
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