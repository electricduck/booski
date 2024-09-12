using System.Diagnostics;
using Booski.Common.Config;
using Booski.Common.Options;
using Booski.Contexts;
using Booski.Helpers;

namespace Booski.Commands;

public interface IStartCommand
{
    StartOptions Options { get; set; }

    Task Invoke(StartOptions o);
}

internal sealed class StartCommand : IStartCommand
{
    public StartOptions Options { get; set; }

    private IBskyContext _bskyContext;
    private IHttpContext _httpContext;
    private IMastodonContext _mastodonContext;
    private IMastodonHelpers _mastodonHelpers;
    private IPostHelpers _postHelpers;
    private ITelegramContext _telegramContext;
    private IXContext _xContext;

    public StartCommand(
        IBskyContext bskyContext,
        IHttpContext httpContext,
        IMastodonContext mastodonContext,
        IMastodonHelpers mastodonHelpers,
        IPostHelpers postHelpers,
        ITelegramContext telegramContext,
        IXContext xContext
    )
    {
        _bskyContext = bskyContext;
        _httpContext = httpContext;
        _mastodonContext = mastodonContext;
        _mastodonHelpers = mastodonHelpers;
        _postHelpers = postHelpers;
        _telegramContext = telegramContext;
        _xContext = xContext;
    }

    public async Task Invoke(StartOptions o)
    {
        if(!o.NoDaemon)
        {
            string currentProcessPath = Program.CurrentProcess.MainModule.FileName;

            if(Pid.GetPid() != null)
            {
                Say.Error("Already running daemon");
            }
            else
            {
                var newProcess = Process.Start(currentProcessPath, $"start --no-daemon --no-say {Program.Arguments}");
                var newProcessPid = newProcess.Id;
                Pid.CreatePid(newProcessPid);

                Say.Success($"Started daemon ({newProcessPid})");
            }

            Program.Exit(kill: true);
        }

        if(o.NoSay)
            Program.NoSay = true;

        await CreateBskyClient(Program.Config.Clients);
#pragma warning disable CS8602 // Dereference of a possibly null reference.
        await CreateAdditionalClients(Program.Config.Clients, o);
#pragma warning restore CS8602 // Dereference of a possibly null reference.

        if(o.ExitAfterConnecting)
            return;

        o.SleepTime = o.SleepTime * 1000;
        o.SleepTimeFetch = o.SleepTimeFetch * 1000;
        o.SleepTimeSync = o.SleepTimeSync * 1000;
        _postHelpers.HornyOnlyOnX = o.HornyOnlyOnX;

        while (true)
        {
            Say.Info($"Fetching posts every {o.SleepTime} seconds", "Change this with --sleep-time/-s (in seconds)");

            _postHelpers.PostCache = await _postHelpers.BuildPostCache(o.SleepTimeFetch);

            await _postHelpers.SyncDeletedPosts(o.SleepTimeSync);
            await _postHelpers.SyncAddedPosts(o.SleepTimeSync, o.RetryIgnoredPosts);

            if (o.ExitAfterRunOnce)
                Program.Exit();
            else
                Thread.Sleep(o.SleepTime);
        }
    }

    private async Task CreateAdditionalClients(ClientsConfig clientsConfig, StartOptions o)
    {
        _httpContext.CreateClient($"Booski/{Program.GetVersion()}");
        _mastodonContext.ResetClient();
        _telegramContext.ResetClient();
        _xContext.ResetClient();

        if (
            !o.NoConnectMastodon &&
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
                Say.Success($"Connected to {_mastodonContext.State.InstanceSoftware}: {_mastodonContext.State.Username} ({_mastodonContext.State.UserId})");
            else
                Say.Warning($"Unable to connect to {_mastodonContext.State.InstanceSoftware}");
        }

        if (
            !o.NoConnectTelegram &&
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

        if(
            !o.NoConnectThreads
        )
        {
        }

        if (
            !o.NoConnectX&&
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