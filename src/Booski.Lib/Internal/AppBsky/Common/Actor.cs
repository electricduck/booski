
namespace Booski.Lib.Internal.AppBsky.Common {
    public class Actor {
        public Associated Associated { get; set; }
        public Uri Avatar { get; set; }
        public string Description { get; set; }
        public string Did { get; set; }
        public string DisplayName { get; set; }
        public string Handle { get; set; }
        public DateTime IndexedAt { get; set; }
        public List<Label> Labels { get; set; }
        public Viewer Viewer { get; set; }
    }
}