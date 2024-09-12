
namespace Booski.Common.Config;

public class BskyConfig
{
    public string Host { get; set; } = "bsky.social";
    public required string Password { get; set; }
    public required string Username { get; set; }
}