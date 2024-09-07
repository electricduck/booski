
namespace Booski.Lib.Internal.AppBsky.Common {
    public class External  {
        public string Description { get; set; }
        public dynamic Thumb { get; set; } // BUG: (Upstream) Sometimes a string, somethings FileBlob. wtf??
        public string Title { get; set; }
        public Uri Uri { get; set; }
    }
}