using Booski.Lib.Internal.ComAtproto.Common;

namespace Booski.Lib.Internal.ComAtproto.Responses {
    public class GetInviteCodesResponse {
        public List<InviteCode> Codes { get; set; }
        public string Cursor { get; set; }
    }
}