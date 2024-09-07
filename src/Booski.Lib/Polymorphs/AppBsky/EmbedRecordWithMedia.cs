using Booski.Lib.Common;

namespace Booski.Lib.Polymorphs.AppBsky {
    // app.bsky.embed.recordWithMedia
    public class EmbedRecordWithMedia : Polymorph {
        public Polymorph Media { get; set; }
        public Polymorph Record { get; set; }
    }
}