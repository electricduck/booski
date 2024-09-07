
namespace Booski.Lib.Internal.ComAtproto.Common {
    public class AccountStatus {
        public bool Activated { get; set; }
        public int ExpectedBlobs { get; set; }
        public int ImportedBlobs { get; set; }
        public int IndexedRecords { get; set; }
        public int PrivateStateValues { get; set; }
        public int RepoBlocks { get; set; }
        public string RepoCommit { get; set; }
        public string RepoRev { get; set; }
        public bool ValidDid { get; set; }
    }
}