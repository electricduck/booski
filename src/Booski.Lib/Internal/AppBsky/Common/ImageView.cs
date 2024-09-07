
using System.Text.Json.Serialization;

namespace Booski.Lib.Internal.AppBsky.Common {
    public class ImageView {
        public string Alt { get; set; }
        public ImageAspectRatio AspectRatio { get; set; }
        public Uri Fullsize { get; set; }
        public Uri Thumb { get; set; }
    }
}