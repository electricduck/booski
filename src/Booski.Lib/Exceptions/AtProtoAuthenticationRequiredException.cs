
namespace Booski.Lib.Exceptions {
    public class AtProtoAuthenticationRequiredException : Exception {
        public AtProtoAuthenticationRequiredException() {}

        public AtProtoAuthenticationRequiredException(string message) : base(message) {}
    }
}