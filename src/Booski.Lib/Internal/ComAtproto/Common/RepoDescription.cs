using Booski.Lib.Common;

namespace Booski.Lib.Internal.ComAtproto.Common {
    public class RepoDescription {
        public string[] Collections { get; set; }
        public string Did { get; set; }
        public DidDoc DidDoc { get; set; }
        public string Handle { get; set; }
        public bool HandleIsCorrect { get; set; }
    }
}