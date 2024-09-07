using System.Text.Json.Serialization;
using ComAtprotoCommon = Booski.Lib.Internal.ComAtproto.Common;

namespace Booski.Lib.Internal.AppBsky.Common {
    public class Image {
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)] 
        public string Alt { get; set; }
        public ImageAspectRatio AspectRatio { get; set; }
        [JsonPropertyName("image")]
        public ComAtprotoCommon.File File { get; set; }
    }
}