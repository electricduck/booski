using Booski.Lib.Common;

namespace Booski.Lib.Polymorphs.AppBsky {
    // app.bsky.actor.defs#savedFeedsPref
    public class ActorSavedFeedsPref : Polymorph {
        public string[] Saved { get; set; }
        public string[] Pinned { get; set; }
    }
}