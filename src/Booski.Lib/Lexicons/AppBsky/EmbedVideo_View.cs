using Booski.Lib.Common;
using Booski.Lib.Internal.AppBsky.Common;

namespace Booski.Lib.Lexicons.AppBsky {
    // app.bsky.embed.video#view
    public class EmbedVideo_View : Lexicon {
        public string Alt { get; set; }
        public AspectRatio AspectRatio { get; set; }
        public Uri Playlist { get; set; }
        public Uri Thumbnail { get; set; }
    }
}