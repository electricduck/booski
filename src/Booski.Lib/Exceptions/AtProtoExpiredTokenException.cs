
namespace Booski.Lib.Exceptions {
    public class AtProtoExpiredTokenException : Exception {
        public AtProtoExpiredTokenException() {}

        public AtProtoExpiredTokenException(string message) : base(message) {}
    }
}