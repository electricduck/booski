using Booski.Common;

namespace Booski.Contexts;

public interface IBskyContext
{
    bool IsConnected { get; set; }
    BskyState? State { get; set; }
}

internal sealed class BskyContext : IBskyContext
{
    public bool IsConnected { get; set; }
    public BskyState? State { get; set; }
}