using Booski.Common.Options;

namespace Booski.Commands;

public interface IMakeConfigCommand
{
    MakeConfigOptions Options { get; set; }

    Task Invoke(MakeConfigOptions o);
}

internal sealed class MakeConfigCommand : IMakeConfigCommand
{
    public MakeConfigOptions Options { get; set; }

    public MakeConfigCommand()
    {

    }

    public async Task Invoke(MakeConfigOptions o)
    {
        Console.WriteLine($"Hello World {o.Overwrite}");
    }
}