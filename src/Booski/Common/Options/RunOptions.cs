using CommandLine;

namespace Booski.Common.Options;

[Verb("run", HelpText = "Run the service.")]
public class RunOptions : GlobalOptions
{
    [Option('s', "sleep-time", Default = 1, HelpText = "Interval (in seconds) between checking for new posts on Bluesky.")]
    public int SleepTime { get; set; }
    [Option("exit-connect", HelpText = "Exit after connecting to services.")]
    public bool ExitAfterConnecting { get; set; }
    [Option("exit-run-once", HelpText = "Exit after first loop.")]
    public bool ExitAfterRunOnce { get; set; }

    [Option("horny-only-x", Hidden = true, HelpText = "Only post sensitive content to X.\nWhat else is this garbage platform even for these days?")]
    public bool HornyOnlyOnX { get; set; }
    [Option("sleep-time-fetch", Default = 1, Hidden = true, HelpText = "Interval (in seconds) between fetching batches of your feed (limited to 100) from Bluesky.")]
    public int SleepTimeFetch { get; set; }
}