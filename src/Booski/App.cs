using System.Reflection;
using System.Runtime.InteropServices;
using System.Web;
using Booski.Common;
using Booski.Contexts;
using Booski.Helpers;
using Booski.Lib;
using BskyApi = Booski.Lib.Lexicon;

namespace Booski;

public interface IApp
{
    Options Options { get; set; }

    Task Run(Options o);
}

internal sealed class App : IApp
{
    public Options Options { get; set; }

    private AtProto _atProto;
    private BskyApi.App.Bsky.Actor _bskyActor;
    private IBskyContext _bskyContext;
    private IGitHubContext _githubContext;
    private IHttpContext _httpContext;
    private IMastodonContext _mastodonContext;
    private IPostHelpers _postHelpers;
    private ITelegramContext _telegramContext;
    private IXContext _xContext;

    public App(
        AtProto atProto,
        BskyApi.App.Bsky.Actor bskyActor,
        IBskyContext bskyContext,
        IGitHubContext githubContext,
        IHttpContext httpContext,
        IMastodonContext mastodonContext,
        IPostHelpers postHelpers,
        ITelegramContext telegramContext,
        IXContext xContext
    )
    {
        _atProto = atProto;
        _bskyActor = bskyActor;
        _bskyContext = bskyContext;
        _githubContext = githubContext;
        _httpContext = httpContext;
        _mastodonContext = mastodonContext;
        _postHelpers = postHelpers;
        _telegramContext = telegramContext;
        _xContext = xContext;
    }

    public async Task Run(Options o)
    {
        Console.WriteLine($"Booski {GetVersion()}");

        if(!o.DoNotCheckForUpdates)
            await CheckUpdates();
        
        await ConfigureApp(o.ConfigPath);
        await CreateBskyClient(o);
        await CreateAdditionalClients(o);

        if (
            _bskyContext.State == null ||
            _bskyContext.State != null &&
            _bskyContext.IsConnected == false
        )
            return;

        o.SleepTime = o.SleepTime * 1000;
        o.SleepTimeFetch = o.SleepTimeFetch * 1000;
        _postHelpers.HornyOnlyOnX = o.HornyOnlyOnX;

        while (true)
        {
            _postHelpers.PostCache = await _postHelpers.BuildPostCache(o.SleepTimeFetch);

            await _postHelpers.SyncDeletedPosts();
            await _postHelpers.SyncAddedPosts();

            if (o.RunOnceThenExit)
                Environment.Exit(0);
            else
                Thread.Sleep(o.SleepTime);
        }
    }

    private async Task CheckUpdates()
    {
        await _githubContext.CreateClient();
        var releases = await _githubContext.Client.Repository.Release.GetAll("electricduck", "booski");
        
        if(releases != null)
        {
            var latestRelease = releases[0];
            var latestTag = latestRelease.TagName;
            var latestVersion = latestTag.Split('/').Last();
            var latestVersionLink = $"https://github.com/electricduck/booski/releases/tag/{HttpUtility.UrlEncode(latestTag)}";
            var runningVersion = GetVersion();

            if(latestVersion != runningVersion)
            {
                Say.Separate();
                Say.Warning("An update is available.", $"Version {latestVersion} is available. Download from {latestVersionLink}.");
                Say.Separate();
            }
        }
    }

    private async Task ConfigureApp(string configPath)
    {
        if (String.IsNullOrEmpty(configPath))
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                var linuxUser = Environment.GetEnvironmentVariable("USER");
                var linuxHome = Environment.GetEnvironmentVariable("HOME");

                if (linuxUser == "root")
                {
                    configPath = "/etc/booski";
                }
                else
                {
                    if (!String.IsNullOrEmpty(linuxHome))
                    {
                        configPath = Path.Combine(linuxHome, ".config/booski");
                    }
                }
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                var windowsLocalAppData = Environment.GetEnvironmentVariable("localappdata");
                if (!String.IsNullOrEmpty(windowsLocalAppData))
                    configPath = Path.Combine(windowsLocalAppData, "booski");
            }

            if (string.IsNullOrEmpty(configPath))
            {
                configPath = Path.Combine(Directory.GetCurrentDirectory(), "config");
            }
        }

        configPath = Path.GetFullPath(configPath);

        Say.Info($"Using config path: {configPath}");

        if (!Directory.Exists(configPath))
            Directory.CreateDirectory(configPath);

        Database.DbPath = configPath;

        Say.Info("Migrating database...");
        Database.Migrate();
    }

    private async Task CreateAdditionalClients(Options o)
    {
        _httpContext.CreateClient($"Booski/{GetVersion()}");
        _mastodonContext.ResetClient();
        _telegramContext.ResetClient();
        _xContext.ResetClient();

        if (
            !String.IsNullOrEmpty(o.MastodonInstance) &&
            !String.IsNullOrEmpty(o.MastodonToken)
        )
        {
            await _mastodonContext.CreateClient(o.MastodonInstance, o.MastodonToken);

            if (_mastodonContext.IsConnected && _mastodonContext.State != null)
                Say.Success($"Connected to Mastodon: {_mastodonContext.State.Username} ({_mastodonContext.State.UserId})");
            else
                Say.Warning("Unable to connect to Mastodon");
        }

        if (
            !String.IsNullOrEmpty(o.TelegramChannel) &&
            !String.IsNullOrEmpty(o.TelegramToken)
        )
        {
            await _telegramContext.CreateClient(o.TelegramToken, o.TelegramChannel);

            if (_telegramContext.IsConnected && _telegramContext.State != null)
                Say.Success($"Connected to Telegram: {_telegramContext.State.Username} ({_telegramContext.State.UserId})");
            else
                Say.Warning("Unable to connect to Telegram");
        }

        if (
            !String.IsNullOrEmpty(o.XAccessToken) &&
            !String.IsNullOrEmpty(o.XAccessSecret) &&
            !String.IsNullOrEmpty(o.XApiKey) &&
            !String.IsNullOrEmpty(o.XApiSecret)
        )
        {
            await _xContext.CreateClient(o.XApiKey, o.XApiSecret, o.XAccessToken, o.XAccessSecret);

            if (_xContext.IsConnected && _xContext.State != null)
                Say.Success($"Connected to X: {_xContext.State.Username}");
            else
                Say.Warning("Unable to connect to X");
        }
    }

    private async Task CreateBskyClient(Options o)
    {
        _bskyContext.State = new BskyState();

        await _atProto.CreateSession(
            o.BskyUsername,
            o.BskyPassword,
            o.BskyHost
        );

        if (_atProto.GetSession() != null)
        {
            var bskyProfileResponse = await _bskyActor.GetProfile(o.BskyUsername);
            _bskyContext.State.Profile = bskyProfileResponse.Data;
            _bskyContext.State.SetAdditionalFields();

            if (!String.IsNullOrEmpty(_bskyContext.State.Did))
            {
                _bskyContext.IsConnected = true;
                Say.Success($"Connected to Bluesky: {_bskyContext.State.Handle} ({_bskyContext.State.Did})");
            }
            else
            {
                _bskyContext.IsConnected = false;
                _bskyContext.State = null;
                Say.Warning("Unable to connect to Bluesky");
            }
        }
    }

    private string GetVersion()
    {
        string versionString = "?.?";

        var entryAssembly = Assembly.GetEntryAssembly();

        if (entryAssembly != null)
        {
            var version = entryAssembly.GetName().Version;

            if (version != null)
            {
                versionString = $"{version.Major}.{version.Minor}";
                if (version.Build > 0)
                    versionString += $".{version.Build}";
            }
        }

        return versionString;
    }
}