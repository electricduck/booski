using Booski.Lib.Enums;

namespace Booski.Lib.Common
{
    public class Session
    {
        public bool Authenticated { get; set; }
        public string Host { get; set; }
        public SessionTokens Tokens { get; set; }
        public SessionType Type { get; set; }
    }
}