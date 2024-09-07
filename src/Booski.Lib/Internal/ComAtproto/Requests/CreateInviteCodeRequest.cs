using System.Text.Json.Serialization;
using Booski.Lib.Common;

namespace Booski.Lib.Internal.ComAtproto.Requests {
    public class CreateInviteCodeRequest {
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)] 
        public string ForAccount { get; set; }
        public int UseCount { get; set; }
    }
}