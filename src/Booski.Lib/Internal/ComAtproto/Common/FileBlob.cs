using File = Booski.Lib.Internal.ComAtproto.Common.File;

namespace Booski.Lib.Internal.ComAtproto.Common {
    public class FileBlob : File {
        public string MimeType { get; set; }
        public Ref Ref { get; set; }
        public int Size { get; set; }
    }
}