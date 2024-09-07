using Booski.Lib.Internal.ComAtproto.Common;

namespace Booski.Lib.Internal.ComAtproto.Responses {
    public class ListRecordsResponse {
        public string Cursor { get; set; }
        public List<RecordMeta> Records { get; set; }
    }
}