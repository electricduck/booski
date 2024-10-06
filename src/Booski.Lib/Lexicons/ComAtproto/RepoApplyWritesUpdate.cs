using Booski.Lib.Common;

namespace Booski.Lib.Lexicons.ComAtproto {
    // com.atproto.repo.applyWrites.update
    public class RepoApplyWritesUpdate : Lexicon {
        public string Collection { get; set; }
        public string RKey { get; set; }
        public string Value { get; set; }
    }
}