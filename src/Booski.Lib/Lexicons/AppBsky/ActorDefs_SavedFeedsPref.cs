using Booski.Lib.Common;

namespace Booski.Lib.Lexicons.AppBsky {
    // app.bsky.actor.defs#savedFeedsPref
    public class ActorDefs_SavedFeedsPref : Lexicon {
        public string[] Saved { get; set; }
        public string[] Pinned { get; set; }
    }
}