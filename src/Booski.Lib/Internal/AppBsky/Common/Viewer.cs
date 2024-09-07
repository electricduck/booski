
namespace Booski.Lib.Internal.AppBsky.Common {
    public class Viewer {
        public bool BlockedBy { get; set; }
        public List BlockedByList { get; set; }
        public string Blocking { get; set; }
        public string FollowedBy { get; set; }
        public string Following { get; set; }
        public bool Muted { get; set; }
        public List MutedByList { get; set; }
    }
}