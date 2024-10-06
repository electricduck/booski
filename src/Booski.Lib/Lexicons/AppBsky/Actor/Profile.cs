using Booski.Lib.Common;

namespace Booski.Lib.Lexicons.AppBsky.Actor {
    // app.bsky.actor.profile
    public class Profile : Lexicon {
        // public ??? Avatar { get; set; } // blob
        // public ??? Banner { get; set; } // blob
        public DateTime CreatedAt { get; set; }
        public string DisplayName { get; set; }
        public ComAtproto.Repo.StrongRef JoinedViaStarterPack { get; set; }
        public List<Internal.AppBsky.Common.Label> Labels { get; set; }
        public ComAtproto.Repo.StrongRef PinnedPost { get; set; }
    }
}