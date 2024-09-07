using Booski.Lib.Common;

namespace Booski.Lib.Polymorphs.ComAtproto {
    // com.atproto.repo.applyWrites.create
    public class RepoApplyWritesCreate : Polymorph {
        public string Collection { get; set; }
        public string RKey { get; set; }
        public string Value { get; set; }
    }
}