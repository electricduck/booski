
using System.Text.Json.Serialization;

namespace Booski.Lib.Internal.ComAtproto.Requests {
    public class DeleteRecordRequest {        
        public string Collection { get; set; }
        public string Repo { get; set; }
        public string RKey { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string SwapCommit { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string SwapRecord { get; set; }
    }
}