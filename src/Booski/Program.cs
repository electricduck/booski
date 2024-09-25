using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Web;
using CommandLine;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Http;
using Booski.Commands;
using Booski.Common.Config;
using Booski.Common.Options;
using Booski.Contexts;
using Booski.Helpers;
using Booski.Lib;

namespace Booski;

public class Program
{
    public static string? Arguments { get; set; }
    public static ConfigRoot? Config { get; set; }
    public static string? ConfigDir { get; set; }
    public static string? ConfigPath { get; set; }
    public static Process? CurrentProcess { get; set; }
    public static string? DbPath { get; set; }
    public static string? FileCacheDir { get; set; }
    public static bool NoSay { get; set; }
    public static string? PidPath { get; set; }
    public static bool YtDlpEnabled { get; set; }
    public static string? YtDlpPath { get; set; } = "yt-dlp";

    private static readonly string DefaultConfigFileContent = """
{
    "Clients": {
        "Bluesky": {
            "Host": "bsky.social",
            "Password": "",
            "Username": ""
        },
        "Mastodon": {
            "Instance": "",
            "Token": ""
        },
        "Nostr": {
            "PrivateKey": "",
            "PublicKey": ""
        },
        "Telegram": {
            "Channel": "",
            "Token": ""
        },
        "X": {
            "AccessSecret": "",
            "AccessToken": "",
            "ApiKey": "",
            "ApiSecret": ""
        }
    }
} 
""";

    public static async Task Main(string[] args)
    {
        Console.OutputEncoding = Encoding.UTF8;
        CurrentProcess = Process.GetCurrentProcess();

        if(
            args.Length > 0 &&
            args[0] == "run"
        )
        {
            args = args.Skip(1).ToArray();
            args = args.Prepend("--no-daemon").Prepend("start").ToArray();
        }

        foreach(var arg in args)
            Arguments+= $"{arg} ";

        try
        {
            HostApplicationBuilder builder = Host.CreateApplicationBuilder();

            builder.Services.AddBooskiLib();
            builder.Services.AddSingleton<II18nHelpers, I18nHelpers>();
            builder.Services.AddSingleton<IBridgyFedHelpers, BridgyFedHelpers>();
            builder.Services.AddSingleton<IBskyContext, BskyContext>();
            builder.Services.AddSingleton<IBskyHelpers, BskyHelpers>();
            builder.Services.AddSingleton<IFileCacheContext, FileCacheContext>();
            builder.Services.AddSingleton<IGitHubContext, GitHubContext>();
            builder.Services.AddSingleton<IHttpContext, HttpContext>();
            builder.Services.AddSingleton<IMastodonContext, MastodonContext>();
            builder.Services.AddSingleton<IMastodonHelpers, MastodonHelpers>();
            builder.Services.AddSingleton<IPostHelpers, PostHelpers>();
            builder.Services.AddSingleton<IStartCommand, StartCommand>();
            builder.Services.AddSingleton<IStatusCommand, StatusCommand>();
            builder.Services.AddSingleton<IStopCommand, StopCommand>();
            builder.Services.AddSingleton<ITelegramContext, TelegramContext>();
            builder.Services.AddSingleton<ITelegramHelpers, TelegramHelpers>();
            builder.Services.AddSingleton<IUsernameMapCommand, UsernameMapCommand>();
            builder.Services.AddSingleton<IYtDlpContext, YtDlpContext>();
            builder.Services.AddSingleton<IXContext, XContext>();
            builder.Services.AddSingleton<IXHelpers, XHelpers>();
            builder.Services.RemoveAll<IHttpMessageHandlerBuilderFilter>();

            using IHost host = builder.Build();

            var _startCommand = host.Services.GetRequiredService<IStartCommand>();
            var _statusCommand = host.Services.GetRequiredService<IStatusCommand>();
            var _stopCommand = host.Services.GetRequiredService<IStopCommand>();
            var _usernameMap = host.Services.GetRequiredService<IUsernameMapCommand>();

            await CheckUpdates(host.Services.GetRequiredService<IGitHubContext>());
            Configure();
            CheckYtDlp(host.Services.GetRequiredService<IYtDlpContext>());
            Database.Migrate();

            await Parser.Default
                .ParseArguments<StartOptions, StopOptions, StatusOptions, UsernameMapOptions>(args)
                .MapResult(
                    (StartOptions o) => _startCommand.Invoke(o),
                    (StopOptions o) => _stopCommand.Invoke(o),
                    (StatusOptions o) => _statusCommand.Invoke(o),
                    (UsernameMapOptions o) => _usernameMap.Invoke(o),
                    errs => Task.FromResult(0)
                );

            Exit();
        }
        catch (Exception e)
        {
            var throwError = Environment.GetEnvironmentVariable("BOOSKI_DEBUG");

#if DEBUG
            throwError = "true";
#endif

            if (throwError == "1" || throwError == "true")
                throw;
            else
            {
                Say.Error(e);
                Exit(true);
            }
        }
    }

    public static void Exit(bool notClean = false, bool kill = false)
    {
        int exitCode = notClean ? 255 : 0;

        Say.Debug($"Exiting ({exitCode})");
        if(kill && Program.CurrentProcess != null)
            Program.CurrentProcess.Kill();
        else
            Environment.Exit(exitCode);
    }

    // TODO: Get tag (-tag)
    public static string GetVersion()
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

    // BUG: Using a tagged version triggers this
    private static async Task CheckUpdates(IGitHubContext _githubContext)
    {
        var ignoreUpdates = Environment.GetEnvironmentVariable("BOOSKI_IGNORE_UPDATES");

        if (ignoreUpdates == "1" || ignoreUpdates == "true")
            return;

        await _githubContext.CreateClient();
#pragma warning disable CS8602 // Dereference of a possibly null reference.
        var releases = await _githubContext.Client.Repository.Release.GetAll("electricduck", "booski");
#pragma warning restore CS8602 // Dereference of a possibly null reference.

        if (releases != null)
        {
            Octokit.Release? latestRelease = null;

            foreach(var release in releases)
            {
                if(!release.Prerelease)
                {
                    latestRelease = release;
                    break;
                }
            }

            if(latestRelease != null)
            {
                var latestTag = latestRelease.TagName;
                var latestVersion = latestTag.Split('/').Last();
                var latestVersionLink = $"https://github.com/electricduck/booski/releases/tag/{HttpUtility.UrlEncode(latestTag)}";
                var runningVersion = GetVersion();

                if (latestVersion != runningVersion)
                {
                    Say.Separate();
                    Say.Warning("An update is available", $"Version {latestVersion} is available. Download from {latestVersionLink}");
                    Say.Separate();
                }
            }
        }

        _githubContext.ResetClient();
    }

    // TODO: Download yt-dlp?
    private static void CheckYtDlp(IYtDlpContext _ytDlpContext)
    {
        var ignoreYtDlp = Environment.GetEnvironmentVariable("BOOSKI_IGNORE_YTDLP");
        var customYtDlpPath = Environment.GetEnvironmentVariable("BOOSKI_YTDLP_PATH");
        if(!String.IsNullOrEmpty(customYtDlpPath))
            YtDlpPath = customYtDlpPath;

        YtDlpEnabled = _ytDlpContext.DoesYtDlpExist();

        if(!YtDlpEnabled)
        {
            if (ignoreYtDlp == "1" || ignoreYtDlp == "true")
                Say.Warning("yt-dlp not found");
            else
                Say.Error(
                    "yt-dlp not found",
                    @$"The ability to download videos from Bluesky depends on this. You must:
* Download yt-dlp (from https://github.com/yt-dlp/yt-dlp)
* Have the binary located in the PATH
 * Or, set this location manually in $BOOSKI_YTDLP_PATH
    "
                );
                Exit(true);
        }
    }

    static void Configure()
    {
        ConfigDir = !String.IsNullOrEmpty(Environment.GetEnvironmentVariable("BOOSKI_CONFIG_DIR")) ?
            Environment.GetEnvironmentVariable("BOOSKI_CONFIG_DIR") :
            Environment.GetEnvironmentVariable("BOOSKI_CONFIG_PATH");

        var configDirSuffix = Environment.GetEnvironmentVariable("BOOSKI_CONFIG_DIR_SUFFIX");

        string appName = "Booski"; // TODO: Get programatically

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            appName = appName.ToLower().Replace(" ", "-");

        try
        {
            if(
                RuntimeInformation.IsOSPlatform(OSPlatform.Linux) &&
                Directory.Exists("/etc/Booski")
            )
                Directory.Move("/etc/Booski", "/etc/booski");
        } catch {}

        if (String.IsNullOrEmpty(ConfigDir))
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                var windowsLocalAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                if (!String.IsNullOrEmpty(windowsLocalAppData))
                    ConfigDir = Path.Combine(windowsLocalAppData, appName);
            }
            else
            {
                var user = Environment.GetEnvironmentVariable("USER");

                if (user == "root")
                {
                    ConfigDir = $"/etc/{appName}";
                }
                else
                {
                    var homeConfDir = "";

                    if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                    {
                        // TODO: Get homedir properly on macOS
                        homeConfDir = $"/Users/{user}/Library/Preferences";
                    }
                    else
                    {
                        var homeDir = Environment.GetEnvironmentVariable("HOME");
                        if (!String.IsNullOrEmpty(homeDir))
                            homeConfDir = Path.Combine(homeDir, ".config");
                    }

                    if (!String.IsNullOrEmpty(homeConfDir))
                        ConfigDir = Path.Combine(homeConfDir, appName);
                }
            }
        }

        if (String.IsNullOrEmpty(ConfigDir))
            ConfigDir = Path.Combine(Directory.GetCurrentDirectory(), "config");

        if (!String.IsNullOrEmpty(configDirSuffix))
            ConfigDir = Path.Combine(ConfigDir, configDirSuffix);

        ConfigDir = Path.GetFullPath(ConfigDir);
        ConfigPath = Path.Combine(ConfigDir, "booski.json");
        DbPath = Path.Combine(ConfigDir, "booski.db");
        FileCacheDir = Path.Combine(ConfigDir, "file-cache");
        PidPath = Path.Combine(ConfigDir, "booski.pid");
        bool firstRun = false;

        if (!Directory.Exists(ConfigDir))
            Directory.CreateDirectory(ConfigDir);

        if(!Directory.Exists(FileCacheDir))
            Directory.CreateDirectory(FileCacheDir);

        if (File.Exists(ConfigPath))
        {
            using StreamReader configFileReader = new(ConfigPath);
            string configFileText = configFileReader.ReadToEnd();
            configFileReader.Close();

            if (configFileText == DefaultConfigFileContent)
                firstRun = true;
        }
        else if (
            !File.Exists(ConfigPath) ||
            new FileInfo(ConfigPath).Length == 0
        )
        {
            firstRun = true;
        }

        if (firstRun)
        {
            File.WriteAllText(ConfigPath, DefaultConfigFileContent);
            Say.Custom("Hey there, seems like you haven't ran Booski before!", $"Edit the config at '{ConfigPath}'", "👋", true);

            Exit();
        }

        IConfiguration config = new ConfigurationBuilder()
            .AddJsonFile(ConfigPath)
            .AddEnvironmentVariables()
            .Build();

        Config = new ConfigRoot();

        var clientsConfigSection = config.GetRequiredSection("Clients");
        Config.Clients = clientsConfigSection.Get<ClientsConfig>();

        // TODO: Improve this!
        if (
            Config.Clients == null ||
            Config.Clients.Bluesky == null ||
            Config.Clients.Bluesky.Password == null ||
            Config.Clients.Bluesky.Username == null
        )
        {
            Say.Error("Required config values are missing");
        }

        if (Config.Clients != null)
        {
            if (Config.Clients.Bluesky != null)
            {
                if (Config.Clients.Bluesky.Host.StartsWith("http://"))
                    Config.Clients.Bluesky.Host.Replace("http://", "");
                if (Config.Clients.Bluesky.Host.StartsWith("https://"))
                    Config.Clients.Bluesky.Host.Replace("https://", "");
            }

            if (Config.Clients.Mastodon != null)
            {
                if (
                    !Config.Clients.Mastodon.Instance.StartsWith("https://") &&
                    !Config.Clients.Mastodon.Instance.StartsWith("http://")
                )
                    Config.Clients.Mastodon.Instance = $"https://{Config.Clients.Mastodon.Instance}";
            }

            if (Config.Clients.Telegram != null)
            {
                if (
                    !Config.Clients.Telegram.Channel.StartsWith("-1") &&
                    !Config.Clients.Telegram.Channel.StartsWith("@")
                )
                    Config.Clients.Telegram.Channel = $"@{Config.Clients.Telegram.Channel}";
            }
        }
    }
}
