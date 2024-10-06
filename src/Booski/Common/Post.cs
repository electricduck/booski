using Booski.Enums;
using AppBskyLexicons = Booski.Lib.Lexicons.AppBsky;

namespace Booski.Common;

public class Post {
    public Language Language { get; set; }
    public AppBskyLexicons.Actor.Defs_ProfileViewDetailed? Profile { get; set; }
    public Booski.Lib.Lexicons.AppBsky.FeedPost? Record { get; set; }
    public string? RecordKey  { get; set; }
    public Sensitivity Sensitivity { get; set; } = Sensitivity.None;
    public string? Uri { get; set; }
}