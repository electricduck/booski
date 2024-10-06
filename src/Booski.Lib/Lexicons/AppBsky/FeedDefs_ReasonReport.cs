using Booski.Lib.Common;
using Booski.Lib.Internal.AppBsky.Common;

namespace Booski.Lib.Lexicons.AppBsky {
    // app.bsky.feed.defs#reasonRepost
    public class FeedDefs_ReasonReport : Lexicon {
        public Booski.Lib.Internal.AppBsky.Common.Actor By { get; set; }
        public DateTime IndexedAt { get; set; }
    }
}