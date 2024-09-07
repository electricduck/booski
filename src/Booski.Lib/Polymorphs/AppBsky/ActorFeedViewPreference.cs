using Booski.Lib.Common;

namespace Booski.Lib.Polymorphs.AppBsky {
    // app.bsky.actor.defs#feedViewPref
    public class ActorFeedViewPref : Polymorph {
        public string Feed { get; set; }
        public bool HideQuotePosts { get; set; }
        public bool HideReplies { get; set; }
        public int HideRepliesByLikeCount { get; set; }
        public bool HideRepliesByUnfollowed { get; set; }
        public bool HideReposts { get; set; }
    }
}