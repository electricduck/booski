namespace Booski.State;
using Booski.Lib.Internal.AppBsky.Common;

// For future usage!
public static class ConnectionState
{
    public static bool IsBskyConnected { get; set; }
    public static bool IsMastodonConnected { get; set; }
    public static bool IsTelegramConnected { get; set; }
    public static bool IsXConnected { get; set; }

    public static Actor BskyActor { get; set; }
    public static Mastonet.Entities.Account MastodonAccount { get; set; }
    public static Mastonet.Entities.InstanceV2 MastodonInstance { get; set; }
    public static Telegram.Bot.Types.User TelegramUser { get; set; }
    public static LinqToTwitter.Account XAccount { get; set; }

    public static void Reset()
    {
        IsBskyConnected = false;
        IsMastodonConnected = false;
        IsTelegramConnected = false;
        BskyActor = null;
        MastodonAccount = null;
        MastodonInstance = null;
        TelegramUser = null;
        XAccount = null;
    }
}