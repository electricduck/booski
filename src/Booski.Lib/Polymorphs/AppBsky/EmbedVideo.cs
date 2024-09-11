using System.Text.Json.Serialization;
using Booski.Lib.Common;
using Booski.Lib.Internal.AppBsky.Common;
using ComAtprotoCommon = Booski.Lib.Internal.ComAtproto.Common;

namespace Booski.Lib.Polymorphs.AppBsky {
    // app.bsky.embed.video
    public class EmbedVideo : Polymorph {
        //public Video Video { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)] 
        public string Alt { get; set; }
        public AspectRatio AspectRatio { get; set; }
        [JsonPropertyName("video")]
        public ComAtprotoCommon.File File { get; set; }
    }
}