using Booski.Lib.Common;

namespace Booski.Lib.Polymorphs.AppBsky {
    // app.bsky.actor.defs#threadViewPref
    public class ActorThreadViewPref : Polymorph {
        public bool PrioritizeFollowedUsers { get; set; }
        public string Sort { get; set; }
    }
}