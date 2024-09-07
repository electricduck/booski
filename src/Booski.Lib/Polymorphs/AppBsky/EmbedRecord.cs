using Booski.Lib.Common;
using Booski.Lib.Internal.AppBsky.Common;

namespace Booski.Lib.Polymorphs.AppBsky {
    // app.bsky.embed.record
    public class EmbedRecord : Polymorph {
        public RecordDetails Record { get; set; }
    }
}