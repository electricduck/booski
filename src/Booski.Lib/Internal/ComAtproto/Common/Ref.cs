using System.Text.Json.Serialization;

namespace Booski.Lib.Internal.ComAtproto.Common {
    public class Ref {
        [JsonPropertyName("$link")]
        public string Link { get; set; }
    }
}