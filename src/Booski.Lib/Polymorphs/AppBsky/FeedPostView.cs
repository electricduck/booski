using Booski.Lib.Common;
using Booski.Lib.Internal.AppBsky.Common;

namespace Booski.Lib.Polymorphs.AppBsky {
    // app.bsky.feed.defs#postView
    public class FeedPostView : Polymorph {
        public Actor Author { get; set; }
        public string Cid { get; set; }
        public Polymorph Embed { get; set; }
        public DateTime IndexedAt { get; set; }
        public List<Label> Labels { get; set; }
        public int LikeCount { get; set; }
        public Polymorph Record { get; set; }
        public int ReplyCount { get; set; }
        public int RespostCount { get; set; }
        public Threadgate Threadgate { get; set; }
        public string Uri { get; set; }
        public Viewer Viewer { get; set; }
    }
}