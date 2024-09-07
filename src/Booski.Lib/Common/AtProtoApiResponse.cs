using System.Net;
using Booski.Lib.Enums;

namespace Booski.Lib.Common {
    public class AtProtoApiResponse<T>
    {
        public T Data { get; set; } = default(T);
        public bool Ok { get; set; }
        public string RawData { get; set; }
        public HttpStatusCode StatusCode { get; set; }
        public TimeSpan TimeTaken { get; set; }
        public HttpContentType Type { get; set; }
    }
}