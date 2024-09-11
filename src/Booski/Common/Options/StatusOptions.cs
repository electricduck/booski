using CommandLine;

namespace Booski.Common.Options;

[Verb("status", HelpText = "Status of the running service.")]
public class StatusOptions : GlobalOptions
{
}