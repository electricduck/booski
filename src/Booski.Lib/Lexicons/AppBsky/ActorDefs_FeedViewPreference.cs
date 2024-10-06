using Booski.Lib.Common;

namespace Booski.Lib.Lexicons.AppBsky {
    // app.bsky.actor.defs#feedViewPref
    public class ActorDefs_FeedViewPref : Lexicon {
        public string Feed { get; set; }
        public bool HideQuotePosts { get; set; }
        public bool HideReplies { get; set; }
        public int HideRepliesByLikeCount { get; set; }
        public bool HideRepliesByUnfollowed { get; set; }
        public bool HideReposts { get; set; }
    }
}