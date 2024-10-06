using Booski.Lib.Common;
using Booski.Lib.Internal.AppBsky.Common;

namespace Booski.Lib.Lexicons.AppBsky {
    // app.bsky.feed.defs#blockedPost
    public class FeedDefs_BlockedPost : Lexicon {
        public BlockedPostAuthor Author { get; set; }
        public bool Blocked { get; set; }
        public string Uri { get; set; }
    }
}