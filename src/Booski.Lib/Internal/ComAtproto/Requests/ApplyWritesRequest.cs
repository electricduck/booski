using Booski.Lib.Common;

namespace Booski.Lib.Internal.ComAtproto.Requests {
    public class ApplyWritesRequest {        
        public string Repo { get; set; }
        public string SwapCommit { get; set; }
        public bool Validate { get; set; }
        public List<Polymorph> Writes { get; set; }
    }
}