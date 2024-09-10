using Booski.Common.Config;
using Booski.Common.Options;
using Booski.Contexts;
using Booski.Helpers;

namespace Booski.Commands;

public interface IRunCommand
{
    RunOptions Options { get; set; }

    Task Invoke(RunOptions o);
}

internal sealed class RunCommand : IRunCommand
{
    public RunOptions Options { get; set; }

    private IBskyContext _bskyContext;
    private IHttpContext _httpContext;
    private IMastodonContext _mastodonContext;
    private IPostHelpers _postHelpers;
    private ITelegramContext _telegramContext;
    private IXContext _xContext;

    public RunCommand(
        IBskyContext bskyContext,
        IHttpContext httpContext,
        IMastodonContext mastodonContext,
        IPostHelpers postHelpers,
        ITelegramContext telegramContext,
        IXContext xContext
    )
    {
        _bskyContext = bskyContext;
        _httpContext = httpContext;
        _mastodonContext = mastodonContext;
        _postHelpers = postHelpers;
        _telegramContext = telegramContext;
        _xContext = xContext;
    }

    public async Task Invoke(RunOptions o)
    {
        await CreateBskyClient(Program.Config.Clients);
#pragma warning disable CS8602 // Dereference of a possibly null reference.
        await CreateAdditionalClients(Program.Config.Clients);
#pragma warning restore CS8602 // Dereference of a possibly null reference.

        if(o.ExitAfterConnecting)
            return;

        o.SleepTime = o.SleepTime * 1000;
        o.SleepTimeFetch = o.SleepTimeFetch * 1000;
        _postHelpers.HornyOnlyOnX = o.HornyOnlyOnX;

        while (true)
        {
            _postHelpers.PostCache = await _postHelpers.BuildPostCache(o.SleepTimeFetch);

            await _postHelpers.SyncDeletedPosts();
            await _postHelpers.SyncAddedPosts();

            if (o.ExitAfterRunOnce)
                Environment.Exit(0);
            else
                Thread.Sleep(o.SleepTime);
        }
    }

    private async Task CreateAdditionalClients(ClientsConfig clientsConfig)
    {
        _httpContext.CreateClient($"Booski/{Program.GetVersion()}");
        _mastodonContext.ResetClient();
        _telegramContext.ResetClient();
        _xContext.ResetClient();

        if (
            clientsConfig.Mastodon != null &&
            !String.IsNullOrEmpty(clientsConfig.Mastodon.Instance) &&
            !String.IsNullOrEmpty(clientsConfig.Mastodon.Token)
        )
        {
            await _mastodonContext.CreateClient(
                clientsConfig.Mastodon.Instance,
                clientsConfig.Mastodon.Token
            );

            if (_mastodonContext.IsConnected && _mastodonContext.State != null)
                Say.Success($"Connected to Mastodon: {_mastodonContext.State.Username} ({_mastodonContext.State.UserId})");
            else
                Say.Warning("Unable to connect to Mastodon");
        }

        if (
            clientsConfig.Telegram != null &&
            !String.IsNullOrEmpty(clientsConfig.Telegram.Channel) &&
            !String.IsNullOrEmpty(clientsConfig.Telegram.Token)
        )
        {
            await _telegramContext.CreateClient(
                clientsConfig.Telegram.Token,
                clientsConfig.Telegram.Channel
            );

            if (_telegramContext.IsConnected && _telegramContext.State != null)
                Say.Success($"Connected to Telegram: {_telegramContext.State.Username} ({_telegramContext.State.UserId})");
            else
                Say.Warning("Unable to connect to Telegram");
        }

        if (
            clientsConfig.X != null &&
            !String.IsNullOrEmpty(clientsConfig.X.AccessToken) &&
            !String.IsNullOrEmpty(clientsConfig.X.AccessSecret) &&
            !String.IsNullOrEmpty(clientsConfig.X.ApiKey) &&
            !String.IsNullOrEmpty(clientsConfig.X.ApiSecret)
        )
        {
            await _xContext.CreateClient(
                clientsConfig.X.ApiKey,
                clientsConfig.X.ApiSecret, 
                clientsConfig.X.AccessToken,
                clientsConfig.X.AccessSecret
            );

            if (_xContext.IsConnected && _xContext.State != null)
                Say.Success($"Connected to X: {_xContext.State.Username}");
            else
                Say.Warning("Unable to connect to X");
        }
    }

    private async Task CreateBskyClient(ClientsConfig clientsConfig)
    {
        await _bskyContext.CreateSession(clientsConfig);

        if (_bskyContext.IsConnected)
        {
            Say.Success($"Connected to Bluesky: {_bskyContext.State.Handle} ({_bskyContext.State.Did})");
        }
        else
        {
            _bskyContext.ClearSession();
            Say.Warning("Unable to connect to Bluesky");
        }
    }
}