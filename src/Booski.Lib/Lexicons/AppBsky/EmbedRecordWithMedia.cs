using Booski.Lib.Common;

namespace Booski.Lib.Lexicons.AppBsky {
    // app.bsky.embed.recordWithMedia
    public class EmbedRecordWithMedia : Lexicon {
        public Lexicon Media { get; set; }
        public Lexicon Record { get; set; }
    }
}