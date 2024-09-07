using System.Text.Json.Serialization;

namespace Booski.Lib.Internal.AppBsky.Common {
    public class Label {
        [JsonPropertyName("cid")]
        public string Cid { get; set; }
        [JsonPropertyName("cts")]
        public DateTime CreatedAt { get; set; }
        [JsonPropertyName("exp")]
        public DateTime ExpiresAt { get; set; }
        [JsonPropertyName("val")]
        public string Name { get; set; }
        [JsonPropertyName("neg")]
        public bool Negation { get; set; }
        [JsonPropertyName("sig")]
        public byte Signature { get; set; }
        [JsonPropertyName("src")]
        public string Source { get; set; }
        [JsonPropertyName("uri")]
        public string Uri { get; set; }
        [JsonPropertyName("ver")]
        public int Version { get; set; }
    }
}