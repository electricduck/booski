using Booski.Lib.Common;

namespace Booski.Lib.Polymorphs.AppBsky {
    // app.bsky.actor.defs#mutedWordsPref
    public class ActorMutedWordsPref : Polymorph {
        public List<ActorMutedWordsPrefItem> Items { get; set; }
    }
}