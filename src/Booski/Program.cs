using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
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
using Booski.Utilities;

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
    public static string? PidLogPath { get; set; }

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

/*
    "Nostr": {
        "PrivateKey": "",
        "Relay": "",
    },
*/

    public static async Task Main(string[] args)
    {
        Say.Debug("Cranking up ol' reliable...");

        if(
            args.Length > 0 &&
            args[0] == "run"
        )
        {
            args = args.Skip(1).ToArray();
            args = args.Prepend("--no-daemon").Prepend("start").ToArray();
        }

        foreach(var arg in args)
        {
            Arguments+= $"{arg} ";
        }

        Say.Debug($"Args: {Arguments}");

        Say.Debug("Getting process...");
        CurrentProcess = Process.GetCurrentProcess();

        try
        {
            Say.Debug("Creating host...");
            HostApplicationBuilder builder = Host.CreateApplicationBuilder();

            Say.Debug("Adding Booski.Lib service...");
            builder.Services.AddBooskiLib();
            Say.Debug("Adding internal services...");
            builder.Services.AddSingleton<II18nHelpers, I18nHelpers>();
            builder.Services.AddSingleton<IBridgyFedHelpers, BridgyFedHelpers>();
            builder.Services.AddSingleton<IBskyContext, BskyContext>();
            builder.Services.AddSingleton<IBskyHelpers, BskyHelpers>();
            builder.Services.AddSingleton<IFileCacheContext, FileCacheContext>();
            builder.Services.AddSingleton<IGitHubContext, GitHubContext>();
            builder.Services.AddSingleton<IHttpContext, HttpContext>();
            builder.Services.AddSingleton<IMastodonContext, MastodonContext>();
            builder.Services.AddSingleton<IMastodonHelpers, MastodonHelpers>();
            builder.Services.AddSingleton<INostrContext, NostrContext>();
            builder.Services.AddSingleton<INostrHelpers, NostrHelpers>();
            builder.Services.AddSingleton<IPostHelpers, PostHelpers>();
            builder.Services.AddSingleton<IStartCommand, StartCommand>();
            builder.Services.AddSingleton<IStatusCommand, StatusCommand>();
            builder.Services.AddSingleton<IStopCommand, StopCommand>();
            builder.Services.AddSingleton<ITelegramContext, TelegramContext>();
            builder.Services.AddSingleton<ITelegramHelpers, TelegramHelpers>();
            builder.Services.AddSingleton<IUsernameMapCommand, UsernameMapCommand>();
            builder.Services.AddSingleton<IXContext, XContext>();
            builder.Services.AddSingleton<IXHelpers, XHelpers>();
            builder.Services.RemoveAll<IHttpMessageHandlerBuilderFilter>();

            Say.Debug("Building host...");
            using IHost host = builder.Build();

            Say.Debug("Resolving command services...");
            var _startCommand = host.Services.GetRequiredService<IStartCommand>();
            var _statusCommand = host.Services.GetRequiredService<IStatusCommand>();
            var _stopCommand = host.Services.GetRequiredService<IStopCommand>();
            var _usernameMap = host.Services.GetRequiredService<IUsernameMapCommand>();

            Say.Debug("Checking for updates...");
            await CheckUpdates(host.Services.GetRequiredService<IGitHubContext>());

            Say.Debug("Configuring...");
            Configure();

            Say.Debug("Migrating database...");
            Database.Migrate();

            Say.Debug("Parsing arguments...");
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
            var throwError = EnvUtilities.GetEnvBool("Debug");

#if DEBUG
            throwError = true;
#endif

            if (throwError)
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
        var ignoreUpdates = EnvUtilities.GetEnvBool("Ignore_Updates");
        if (ignoreUpdates)
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

    static void Configure()
    {
        ConfigDir = !String.IsNullOrEmpty(EnvUtilities.GetEnvString("CONFIG_DIR")) ?
            EnvUtilities.GetEnvString("CONFIG_DIR") :
            EnvUtilities.GetEnvString("CONFIG_PATH");

        var configDirSuffix = EnvUtilities.GetEnvString("CONFIG_DIR_SUFFIX");

        string appName = "Booski"; // TODO: Get programatically

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            appName = appName.ToLower().Replace(" ", "-");

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
                var user = EnvUtilities.GetEnvString("USER", false);

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
                        var homeDir = EnvUtilities.GetEnvString("HOME", false);
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
        PidLogPath = Path.Combine(ConfigDir, "booski.log");
        bool firstRun = false;

        string debugOutputBody = $@"↳ Config:     {ConfigPath}
↳ Database:   {DbPath}
↳ File Cache: {FileCacheDir}/
↳ Log:        {PidLogPath}
↳ PID:        {PidPath}
";

        Say.Debug("Computed paths", debugOutputBody);

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
