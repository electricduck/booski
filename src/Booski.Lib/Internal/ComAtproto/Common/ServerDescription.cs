
namespace Booski.Lib.Internal.ComAtproto.Common {
    public class ServerDescription {
        public string[]? AvailableUserDomains { get; set; }
        public ServerDescriptionContact Contact { get; set; }
        public string Did { get; set; }
        public bool InviteCodeRequired { get; set; }
        public ServerDescriptionLinks Links { get; set; }
        public bool PhoneVerificationRequired { get; set; }
    }
}