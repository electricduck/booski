namespace Booski.Common;

public class Post {
    public Booski.Lib.Internal.AppBsky.Common.Actor Profile { get; set; }
    public Booski.Lib.Polymorphs.AppBsky.FeedPost Record { get; set; }
    public string RecordKey  { get; set; }
    public bool Sensitive { get; set; }
    public string Uri { get; set; }
}