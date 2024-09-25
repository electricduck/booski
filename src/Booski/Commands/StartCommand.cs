using System.Diagnostics;
using Booski.Common.Config;
using Booski.Common.Options;
using Booski.Contexts;
using Booski.Helpers;
using Nostr.Client.Client;
using Nostr.Client.Communicator;
using Nostr.Client.Keys;
using Nostr.Client.Messages;
using Nostr.Client.Requests;

namespace Booski.Commands;

public interface IStartCommand
{
    StartOptions? Options { get; set; }

    Task Invoke(StartOptions o);
}

internal sealed class StartCommand : IStartCommand
{
    public StartOptions? Options { get; set; }

    private DateTime? CacheLastCleared { get; set; }

    private IBskyContext _bskyContext;
    private IFileCacheContext _fileCacheContext;
    private IHttpContext _httpContext;
    private IMastodonContext _mastodonContext;
    private INostrContext _nostrContext;
    private IPostHelpers _postHelpers;
    private ITelegramContext _telegramContext;
    private IXContext _xContext;

    public StartCommand(
        IBskyContext bskyContext,
        IFileCacheContext fileCacheContext,
        IHttpContext httpContext,
        IMastodonContext mastodonContext,
        INostrContext nostrContext,
        IPostHelpers postHelpers,
        ITelegramContext telegramContext,
        IXContext xContext
    )
    {
        _bskyContext = bskyContext;
        _fileCacheContext = fileCacheContext;
        _httpContext = httpContext;
        _mastodonContext = mastodonContext;
        _nostrContext = nostrContext;
        _postHelpers = postHelpers;
        _telegramContext = telegramContext;
        _xContext = xContext;
    }

    public async Task Invoke(StartOptions o)
    {
        if (!o.NoDaemon)
        {
            Process? currentProcess = Program.CurrentProcess;
            ProcessModule? currentProcessModule;
            string currentProcessPath = String.Empty;

            if (currentProcess != null)
            {
                currentProcessModule = currentProcess.MainModule;
                if (currentProcessModule != null)
                    currentProcessPath = currentProcessModule.FileName;
            }

            if (String.IsNullOrEmpty(currentProcessPath))
            {
                Say.Error("Unable to run as daemon");
                Program.Exit();
            }

            if (Pid.GetPid() != null)
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

        if (o.NoSay)
            Program.NoSay = true;

#pragma warning disable CS8602 // Dereference of a possibly null reference.
        await CreateBskyClient(Program.Config.Clients);
#pragma warning restore CS8602 // Dereference of a possibly null reference.
#pragma warning disable CS8602 // Dereference of a possibly null reference.
        await CreateAdditionalClients(Program.Config.Clients, o);
#pragma warning restore CS8602 // Dereference of a possibly null reference.

        if (o.ExitAfterConnecting)
            return;

        o.SleepTime = o.SleepTime * 1000;
        o.SleepTimeFetch = o.SleepTimeFetch * 1000;
        o.SleepTimeSync = o.SleepTimeSync * 1000;
        _postHelpers.HornyOnlyOnX = o.HornyOnlyOnX;

        var sleepTimeSeconds = o.SleepTime / 1000;
        var sleepTimeSecondsUnit = sleepTimeSeconds == 1 ? "second" : "seconds";
        Say.Info($"Fetching posts every {sleepTimeSeconds} {sleepTimeSecondsUnit}", "Change this with --sleep-time/-s (in seconds)");

        while (true)
        {
            try
            {
                await _postHelpers.BuildPostCache(o.SleepTimeFetch);
            }
            catch (Exception e)
            {
                // TODO: Handle different exceptions appropriately
                Say.Warning("Unable to cache posts", e.Message);
            }
            finally
            {
                await _postHelpers.SyncDeletedPosts(o.SleepTimeSync);
                await _postHelpers.SyncAddedPosts(o.SleepTimeSync, o.RetryIgnoredPosts);
            }

            if (
                CacheLastCleared != null &&
                DateTime.UtcNow > CacheLastCleared.Value.Date.AddHours(1)
            )
            {
                await _fileCacheContext.ClearCache();
                CacheLastCleared = DateTime.UtcNow;
            }

            if (o.ExitAfterRunOnce)
                Program.Exit();
            else
                Thread.Sleep(o.SleepTime);
        }
    }

    // TODO: Maintain connections?
    //       Note sure if we even need to bother. If a post fails due to a
    //       busted client, it'll just try again later.
    //       This has been tested in practice with Mastodon, Telegram and X;
    //       Nostr needs investigating (this uses WS not HTTP).
    //
    // TODO: Say why clients were unable to be connected to
    private async Task CreateAdditionalClients(ClientsConfig clientsConfig, StartOptions o)
    {
        _httpContext.CreateClient($"Booski/{Program.GetVersion()}");
        _mastodonContext.ResetClient();
        _telegramContext.ResetClient();
        _xContext.ResetClient();

        if (
            !o.NoConnectMastodon &&
            clientsConfig.Mastodon != null &&
            !String.IsNullOrEmpty(clientsConfig.Mastodon.Instance)
        )
        {
            if(!String.IsNullOrEmpty(clientsConfig.Mastodon.Token))
            {
                Say.Debug("Connecting to Mastodon (Mastonet) (with token)...");
                await _mastodonContext.CreateClient(
                    clientsConfig.Mastodon.Instance,
                    clientsConfig.Mastodon.Token
                );
            }
            else if(
                !String.IsNullOrEmpty(clientsConfig.Mastodon.Password) &&
                !String.IsNullOrEmpty(clientsConfig.Mastodon.Username)
            )
            {
                Say.Debug("Connecting to Mastodon (Mastonet) (with username/password)...");
                await _mastodonContext.CreateClient(
                    clientsConfig.Mastodon.Instance,
                    clientsConfig.Mastodon.Password,
                    clientsConfig.Mastodon.Username
                );
            }

            if (_mastodonContext.IsConnected && _mastodonContext.State != null)
                Say.Success($"Connected to {_mastodonContext.State.InstanceSoftware}: {_mastodonContext.State.Username} ({_mastodonContext.State.UserId})");
            else
                Say.Warning($"Unable to connect to Mastodon");
        }

        if (
            !o.NoConnectNostr &&
            clientsConfig.Nostr != null &&
            !String.IsNullOrEmpty(clientsConfig.Nostr.PrivateKey) &&
            !String.IsNullOrEmpty(clientsConfig.Nostr.Relay)
        )
        {
            Say.Debug("Connecting to Nostr (Nostr.Client)...");
            await _nostrContext.CreateClient(
                clientsConfig.Nostr.PrivateKey,
                new string[] { clientsConfig.Nostr.Relay }
            );

            if (_nostrContext.IsConnected && _nostrContext.State != null)
                Say.Success($"Connected to Nostr: {_nostrContext.State.PublicKey} ({clientsConfig.Nostr.Relay})");
            else
                Say.Warning($"Unable to connect to Nostr");
        }

        if (
            !o.NoConnectTelegram &&
            clientsConfig.Telegram != null &&
            !String.IsNullOrEmpty(clientsConfig.Telegram.Channel) &&
            !String.IsNullOrEmpty(clientsConfig.Telegram.Token)
        )
        {
            Say.Debug("Connecting to Telegram (Telegram.Bot)...");
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
            !o.NoConnectX &&
            clientsConfig.X != null &&
            !String.IsNullOrEmpty(clientsConfig.X.AccessToken) &&
            !String.IsNullOrEmpty(clientsConfig.X.AccessSecret) &&
            !String.IsNullOrEmpty(clientsConfig.X.ApiKey) &&
            !String.IsNullOrEmpty(clientsConfig.X.ApiSecret)
        )
        {
            Say.Debug("Connecting to X (LinqToTwitter)...");
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
        Say.Debug("Connecting to Bluesky...");
        await _bskyContext.CreateSession(clientsConfig);

        if (_bskyContext.IsConnected && _bskyContext.State != null)
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