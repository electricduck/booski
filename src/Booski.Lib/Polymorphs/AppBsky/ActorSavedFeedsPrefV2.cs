using Booski.Lib.Common;

namespace Booski.Lib.Polymorphs.AppBsky {
    // app.bsky.actor.defs#savedFeedsPrefV2
    public class ActorSavedFeedsPrefV2 : Polymorph {
        public List<ActorSavedFeedPrefV2Item> Items { get; set; }
    }
}