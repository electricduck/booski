using Booski.Common.Options;

namespace Booski.Commands;

public interface IUsernameMapCommand
{
    UsernameMapOptions Options { get; set; }

    Task Invoke(UsernameMapOptions o);
}

internal sealed class UsernameMapCommand : IUsernameMapCommand
{
    public UsernameMapOptions Options { get; set; }

    public UsernameMapCommand()
    {

    }

    public async Task Invoke(UsernameMapOptions o)
    {
        Console.WriteLine($"Coming soon!");
    }
}