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

namespace Booski;

public class Program
{
    public static string[] Arguments { get; set; }
    public static ConfigRoot? Config { get; set; }
    public static string ConfigDir { get; set; }
    public static string ConfigPath { get; set; }
    public static string DbPath { get; set; }

    private static readonly string DefaultConfigFileContent = """
{
    "Clients": {
        "Bluesky": {
            "Host": "public.apk.bsky.app",
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

    public static async Task Main(string[] args)
    {
        Arguments = args;

#pragma warning disable CS0168 // Variable is declared but never used
        try
        {
            HostApplicationBuilder builder = Host.CreateApplicationBuilder();

            builder.Services.AddBooskiLib();
            builder.Services.AddSingleton<IBskyContext, BskyContext>();
            builder.Services.AddSingleton<IBskyHelpers, BskyHelpers>();
            builder.Services.AddSingleton<IFileCacheContext, FileCacheContext>();
            builder.Services.AddSingleton<IGitHubContext, GitHubContext>();
            builder.Services.AddSingleton<IHttpContext, HttpContext>();
            builder.Services.AddSingleton<IMastodonContext, MastodonContext>();
            builder.Services.AddSingleton<IMastodonHelpers, MastodonHelpers>();
            builder.Services.AddSingleton<IPostHelpers, PostHelpers>();
            builder.Services.AddSingleton<IRunCommand, RunCommand>();
            builder.Services.AddSingleton<ITelegramContext, TelegramContext>();
            builder.Services.AddSingleton<ITelegramHelpers, TelegramHelpers>();
            builder.Services.AddSingleton<IUsernameMapCommand, UsernameMapCommand>();
            builder.Services.AddSingleton<IXContext, XContext>();
            builder.Services.AddSingleton<IXHelpers, XHelpers>();
            builder.Services.RemoveAll<IHttpMessageHandlerBuilderFilter>();

            using IHost host = builder.Build();

            var _run = host.Services.GetRequiredService<IRunCommand>();
            var _usernameMap = host.Services.GetRequiredService<IUsernameMapCommand>();
            
            await CheckUpdates(host.Services.GetRequiredService<IGitHubContext>());
            Configure();
            Database.Migrate();

            await Parser.Default
                .ParseArguments<RunOptions, UsernameMapOptions>(args)
                .MapResult(
                    (RunOptions o) => _run.Invoke(o),
                    (UsernameMapOptions o) => _usernameMap.Invoke(o),
                    errs => Task.FromResult(0)
                );

            Exit();
        }
        catch (Exception e)
        {
#if DEBUG
                throw;
#else
                Say.Error(e);
#endif

#pragma warning disable CS0162 // Unreachable code detected
            Exit(1);
#pragma warning restore CS0162 // Unreachable code detected
        }
#pragma warning restore CS0168 // Variable is declared but never used
    }

    public static void Exit(int exitCode = 0)
    {
        Say.Debug($"Exiting ({exitCode})");
        Environment.Exit(exitCode);
    }

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

    private static async Task CheckUpdates(IGitHubContext _githubContext)
    {
        var ignoreUpdates = Environment.GetEnvironmentVariable("BOOSKI_IGNORE_UPDATES");

        if(ignoreUpdates == "1" || ignoreUpdates == "true")
            return;

        await _githubContext.CreateClient();
#pragma warning disable CS8602 // Dereference of a possibly null reference.
        var releases = await _githubContext.Client.Repository.Release.GetAll("electricduck", "booski");
#pragma warning restore CS8602 // Dereference of a possibly null reference.

        if (releases != null)
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

        _githubContext.ResetClient();
    }

    static void Configure()
    {
        ConfigDir = Environment.GetEnvironmentVariable("BOOSKI_CONFIG_PATH");

        if (String.IsNullOrEmpty(ConfigDir))
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                var linuxUser = Environment.GetEnvironmentVariable("USER");
                var linuxHome = Environment.GetEnvironmentVariable("HOME");

                if (linuxUser == "root")
                {
                    ConfigDir = "/etc/booski";
                }
                else
                {
                    if (!String.IsNullOrEmpty(linuxHome))
                    {
                        ConfigDir = Path.Combine(linuxHome, ".config", "booski");
                    }
                }
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                var windowsLocalAppData = Environment.SpecialFolder.LocalApplicationData.ToString();
                if (!String.IsNullOrEmpty(windowsLocalAppData))
                    ConfigDir = Path.Combine(windowsLocalAppData, "Booski");
            }

            if (string.IsNullOrEmpty(ConfigDir))
            {
                ConfigDir = Path.Combine(Directory.GetCurrentDirectory(), "config");
            }

            ConfigDir = Path.GetFullPath(ConfigDir);
            ConfigPath = Path.Combine(ConfigDir, "booski.json");
            DbPath = Path.Combine(ConfigDir, "booski.db");
            bool firstRun = false;

            if (!Directory.Exists(ConfigDir))
                Directory.CreateDirectory(ConfigDir);

            if(File.Exists(ConfigPath))
            {
                using StreamReader configFileReader = new(ConfigPath);
                string configFileText = configFileReader.ReadToEnd();

                if(configFileText == DefaultConfigFileContent)
                    firstRun = true;
            }
            else if(
                !File.Exists(ConfigPath) ||
                new FileInfo(ConfigPath).Length == 0 
            )
            {
                firstRun = true;
            }

            if(firstRun)
            {
                File.WriteAllText(ConfigPath, DefaultConfigFileContent);
                Say.Custom("Hey there, seems like you haven't ran Booski before!", $"Edit the config at '{ConfigPath}'.", "👋", true);
                
                Exit(1);
            }

            IConfiguration config = new ConfigurationBuilder()
                .AddJsonFile(ConfigPath)
                .AddEnvironmentVariables()
                .Build();

            Config = new ConfigRoot();

            var clientsConfigSection = config.GetRequiredSection("Clients");

            if(clientsConfigSection != null)
                Config.Clients = clientsConfigSection.Get<ClientsConfig>();

            if(Config.Clients != null)
            {
                if(Config.Clients.Bluesky != null)
                {
                    if(Config.Clients.Bluesky.Host.StartsWith("http://"))
                        Config.Clients.Bluesky.Host.Replace("http://", "");
                    if(Config.Clients.Bluesky.Host.StartsWith("https://"))
                        Config.Clients.Bluesky.Host.Replace("https://", "");
                }

                if(Config.Clients.Mastodon != null)
                {
                    if(
                        !Config.Clients.Mastodon.Instance.StartsWith("https://") &&
                        !Config.Clients.Mastodon.Instance.StartsWith("http://")
                    )
                        Config.Clients.Mastodon.Instance = $"https://{Config.Clients.Mastodon.Instance}";
                }

                if(Config.Clients.Telegram != null)
                {
                    if(
                        !Config.Clients.Telegram.Channel.StartsWith("-1") &&
                        !Config.Clients.Telegram.Channel.StartsWith("@")
                    )
                        Config.Clients.Telegram.Channel = $"@{Config.Clients.Telegram.Channel}";
                }
            }
        }
    }
}