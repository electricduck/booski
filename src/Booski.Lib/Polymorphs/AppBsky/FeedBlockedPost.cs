using Booski.Lib.Common;
using Booski.Lib.Internal.AppBsky.Common;

namespace Booski.Lib.Polymorphs.AppBsky {
    // app.bsky.feed.defs#blockedPost
    public class FeedBlockedPost : Polymorph {
        public BlockedPostAuthor Author { get; set; }
        public bool Blocked { get; set; }
        public string Uri { get; set; }
    }
}