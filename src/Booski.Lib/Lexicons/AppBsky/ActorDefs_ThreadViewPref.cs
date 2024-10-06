using Booski.Lib.Common;

namespace Booski.Lib.Lexicons.AppBsky {
    // app.bsky.actor.defs#threadViewPref
    public class ActorDefs_ThreadViewPref : Lexicon {
        public bool PrioritizeFollowedUsers { get; set; }
        public string Sort { get; set; }
    }
}