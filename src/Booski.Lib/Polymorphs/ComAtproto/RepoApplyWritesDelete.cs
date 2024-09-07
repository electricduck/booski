using Booski.Lib.Common;

namespace Booski.Lib.Polymorphs.ComAtproto {
    // com.atproto.repo.applyWrites.delete
    public class RepoApplyWritesDelete : Polymorph {
        public string Collection { get; set; }
        public string RKey { get; set; }
    }
}