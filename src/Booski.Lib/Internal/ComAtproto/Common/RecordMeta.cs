using Booski.Lib.Common;

namespace Booski.Lib.Internal.ComAtproto.Common {
    public class RecordMeta {
        public string Cid { get; set; }
        public string Uri { get; set; }
        public Lexicon Value { get; set; }
        //public dynamic Value { get; set; } // NOTE: JSON polymorphism is busted in .NET 8.0 (and below): https://github.com/dotnet/runtime/issues/72604#issuecomment-1440708052
    }
}