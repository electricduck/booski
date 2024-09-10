
namespace Booski.Common.Config;

public class ClientsConfig
{
    public BskyConfig Bluesky { get; set; }
    public MastodonConfig Mastodon { get; set; }
    public TelegramConfig Telegram { get; set; }
    public XConfig X { get; set; }
}