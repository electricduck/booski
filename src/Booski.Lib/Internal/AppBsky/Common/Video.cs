using System.Text.Json.Serialization;
using ComAtprotoCommon = Booski.Lib.Internal.ComAtproto.Common;

namespace Booski.Lib.Internal.AppBsky.Common {
    public class Video {
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)] 
        public string Alt { get; set; }
        public AspectRatio AspectRatio { get; set; }
        [JsonPropertyName("video")]
        public ComAtprotoCommon.File File { get; set; }
    }
}