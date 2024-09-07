using System.Text.Json.Serialization;

namespace Booski.Lib.Internal.ComAtproto.Requests {
    public class PutRecordRequest {
        public string Collection { get; set; }
        public dynamic Record { get; set; }
        public string Repo { get; set; }
        [JsonPropertyName("rkey")]
        public string RKey { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string SwapCommit { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string SwapRecord { get; set; }
        public bool Validate { get; set; }
    }
}