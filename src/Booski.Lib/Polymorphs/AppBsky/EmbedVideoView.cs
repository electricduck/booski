using Booski.Lib.Common;
using Booski.Lib.Internal.AppBsky.Common;

namespace Booski.Lib.Polymorphs.AppBsky {
    // app.bsky.embed.video#view
    public class EmbedVideoView : Polymorph {
        public string Alt { get; set; }
        public AspectRatio AspectRatio { get; set; }
        public Uri Playlist { get; set; }
        public Uri Thumbnail { get; set; }
    }
}