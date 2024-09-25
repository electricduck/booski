namespace Booski.Common;
using System.ComponentModel.DataAnnotations;

public class UsernameMap {
    [Key]
    public string Bluesky_Did { get; set; } = "";

    public string? Mastodon_Handle { get; set; }
    public string? Nostr_Handle { get; set; }
    public string? Telegram_Handle { get; set; }
    public string? Threads_Handle { get; set; } // Note: Futureproofing. Threads is not a priority.
    public string? X_Handle { get; set; }
}