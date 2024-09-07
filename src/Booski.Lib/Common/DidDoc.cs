
using System.Text.Json.Serialization;

namespace Booski.Lib.Common {
    public class DidDoc {
        [JsonPropertyName("@context")]
        public string[] Context { get; set; }
        public string[] AlsoKnownAs { get; set; }
        public string Id { get; set; }
        public List<DidDocService> Service { get; set; }
        public List<DidDocVerificationMethod> VerificationMethod { get; set; }
    }
}