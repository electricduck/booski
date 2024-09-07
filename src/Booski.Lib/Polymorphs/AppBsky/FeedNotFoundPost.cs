using Booski.Lib.Common;

namespace Booski.Lib.Polymorphs.AppBsky {
    // app.bsky.feed.defs#notFoundPost
    public class FeedNotFoundPost : Polymorph {
        public bool NotFound { get; set; }
        public string Uri { get; set; }
    }
}