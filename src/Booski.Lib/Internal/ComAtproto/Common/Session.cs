using Booski.Lib.Common;

namespace Booski.Lib.Internal.ComAtproto.Common {
    public class Session {
        public string AccessJwt { get; set; }
        public string Did { get; set; }
        public DidDoc DidDoc { get; set; }
        public string Email { get; set; }
        public bool EmailAuthFactor { get; set; }
        public bool EmailConfirmed { get; set; }
        public string Handle { get; set; }
        public string RefreshJwt { get; set; }
    }
}