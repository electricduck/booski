namespace Booski.Common;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class PostLog {
    [Key]
    public string RecordKey { get; set; }

    public string? Repository { get; set; }

    public string? Mastodon_InstanceDomain { get; set; }
    public string? Mastodon_StatusId { get; set; }

    public long? Telegram_ChatId { get; set; }
    public int? Telegram_MessageCount { get; set; }
    public int? Telegram_MessageId { get; set; }

    public string? X_PostId { get; set; }

    public bool Deleted { get; set;}
    public bool Ignored { get; set; }
    public DateTime UpdatedAt { get; set; }
    public int Version { get; set; }
}