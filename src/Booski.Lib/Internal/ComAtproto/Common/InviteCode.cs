
namespace Booski.Lib.Internal.ComAtproto.Common {
    public class InviteCode {
        public int Available { get; set; }
        public string Code { get; set; }
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; }
        public bool Disabled { get; set; }
        public string ForAccount { get; set; }
        public List<InviteCodeUse> Uses { get; set; }
    }
}