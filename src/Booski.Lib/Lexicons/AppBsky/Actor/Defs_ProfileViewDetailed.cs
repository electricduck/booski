using System.Text.Json.Serialization;
using Booski.Lib.Common;
using Booski.Lib.Lexicons;

namespace Booski.Lib.Lexicons.AppBsky.Actor {
    // TODO: Refactor the whole thing (woo!)
    // app.bsky.actor.defs#profileViewDetailed
    public class Defs_ProfileViewDetailed : Lexicon {
        public Internal.AppBsky.Common.Associated Associated { get; set; }
        [JsonPropertyName("avatar")]
        public Uri AvatarUri { get; set; }
        [JsonPropertyName("banner")]
        public Uri BannerUri { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Description { get; set; }
        public string Did { get; set; }
        public string DisplayName { get; set; }
        public int FollowersCount { get; set; }
        public int FollowsCount { get; set; }
        public string Handle { get; set; }
        public DateTime IndexedAt { get; set; }
        //public AppBsky.Graph.Defs_StarterPackViewBasic JoinedViaStarterPack { get; set; } // app.bsky.graph.defs#starterPackViewBasic 
        public List<Internal.AppBsky.Common.Label> Labels { get; set; }
        public ComAtproto.Repo.StrongRef PinnedPost { get; set; }
        public Internal.AppBsky.Common.Viewer Viewer { get; set; }
    }
}