using Booski.Enums;

namespace Booski.Common;

public class Post {
    public Language Language { get; set; }
    public Booski.Lib.Internal.AppBsky.Common.Actor? Profile { get; set; }
    public Booski.Lib.Polymorphs.AppBsky.FeedPost? Record { get; set; }
    public string? RecordKey  { get; set; }
    public Sensitivity Sensitivity { get; set; } = Sensitivity.None;
    public string? Uri { get; set; }
}