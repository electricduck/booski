using Booski.Lib.Internal.ComAtproto.Common;

namespace Booski.Lib.Internal.ComAtproto.Responses {
    public class ListMissingBlobsReponse {
        public string Cursor { get; set; }
        public List<BlobMeta> Blobs { get; set; }
    }
}