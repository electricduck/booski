namespace Booski.Common;
using System.ComponentModel.DataAnnotations;

public class UsernameMap {
    [Key]
    public string Bluesky_Did { get; set; }

    public string Mastodon_Handle { get; set; }
    public string Telegram_Handle { get; set; }
    public string X_Handle { get; set; } // NOTE: Futureproofing. X support is not a priority
}