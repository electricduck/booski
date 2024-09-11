using CommandLine;

namespace Booski.Common.Options;

[Verb("stop", HelpText = "Stop the running service.")]
public class StopOptions : GlobalOptions
{
}