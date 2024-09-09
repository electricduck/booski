
namespace Booski.Common.Config;

public class BskyConfig
{
    public string Host { get; set; } = "public.api.bsky.app";
    public required string Password { get; set; }
    public required string Username { get; set; }
}