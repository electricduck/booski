using Booski.Lib.Common;

namespace Booski.Lib.Polymorphs.AppBsky {
    // app.bsky.actor.defs#hiddenPostsPref
    public class ActorHiddenPostsPref : Polymorph {
        public string[] Items { get; set; }
    }
}