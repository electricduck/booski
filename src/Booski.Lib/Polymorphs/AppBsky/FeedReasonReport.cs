using Booski.Lib.Common;
using Booski.Lib.Internal.AppBsky.Common;

namespace Booski.Lib.Polymorphs.AppBsky {
    // app.bsky.feed.defs#reasonRepost
    public class FeedReasonReport : Polymorph {
        public Actor By { get; set; }
        public DateTime IndexedAt { get; set; }
    }
}