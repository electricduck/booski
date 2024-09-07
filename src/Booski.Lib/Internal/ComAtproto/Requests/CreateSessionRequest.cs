
namespace Booski.Lib.Internal.ComAtproto.Requests {
    public class CreateSessionRequest {        
        public string AuthFactorToken { get; set; }
        public string Identifier { get; set; }
        public string Password { get; set; }
    }
}