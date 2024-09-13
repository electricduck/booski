using CommandLine;

namespace Booski.Common.Options;

[Verb("start", HelpText = "Start the service.\nUse --no-daemon to not run as a daemon.")]
public class StartOptions : GlobalOptions
{
    [Option('s', "sleep-time", Default = 1, HelpText = "Interval (in seconds) between checking for new posts on Bluesky.")]
    public int SleepTime { get; set; }
    [Option('n', "no-daemon", HelpText = "Do not run service as daemon.")]
    public bool NoDaemon { get; set; }
    [Option("retry-ignored", HelpText = "Retry posts that have been logged but ignored (does not include posts logged before first run).")]
    public bool RetryIgnoredPosts { get; set; }
    [Option("no-connect-mastodon", HelpText = "Do not connect to Mastodon.")]
    public bool NoConnectMastodon { get; set; }
    [Option("no-connect-telegram", HelpText = "Do not connect to Telegram.")]
    public bool NoConnectTelegram { get; set; }
    [Option("no-connect-x", HelpText = "Do not connect to X.")]
    public bool NoConnectX { get; set; }
    [Option("exit-connect", HelpText = "Exit after connecting to services.")]
    public bool ExitAfterConnecting { get; set; }
    [Option("exit-run-once", HelpText = "Exit after first loop.")]
    public bool ExitAfterRunOnce { get; set; }

    [Option("sleep-time-fetch", Default = 1, Hidden = true, HelpText = "Interval (in seconds) between fetching batches of your feed (limited to 100) from Bluesky.")]
    public int SleepTimeFetch { get; set; }
    [Option("sleep-time-sync", Default = 1, Hidden = true, HelpText = "Interval (in seconds) between syncing individual posts to services.")]
    public int SleepTimeSync { get; set; }
    [Option("horny-only-x", Hidden = true, HelpText = "Only post sensitive content to X.\nWhat else is this garbage platform even for these days?")]
    public bool HornyOnlyOnX { get; set; }
    [Option("no-say", Hidden = true, HelpText = "Do not output anything.")]
    public bool NoSay { get; set; }
}