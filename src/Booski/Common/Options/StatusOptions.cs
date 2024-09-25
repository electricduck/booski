using CommandLine;

namespace Booski.Common.Options;

[Verb("status", HelpText = "Status of the running service.")]
public class StatusOptions : GlobalOptions
{
    [Option('n', "log-lines", Default = 10, HelpText = "Amount of log lines to print.")]
    public int LogLines { get; set; }
    [Option("no-log", HelpText = "Do not print log.")]
    public bool NoLog { get; set; }
}