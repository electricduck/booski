using CommandLine;

namespace Booski.Common.Options;

[Verb("make-config", HelpText = "Make the config!")]
public class MakeConfigOptions : GlobalOptions
{
    [Option("overwrite", HelpText = "??")]
    public bool Overwrite { get; set; }
}