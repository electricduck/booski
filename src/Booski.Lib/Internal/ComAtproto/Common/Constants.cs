using Booski.Lib.Internal.ComAtproto.Enums;

namespace Booski.Lib.Internal.ComAtproto.Common {
    public static class Constants {
        public const int DefaultInviteCodeLimit = 100;
        public const InviteCodeSort DefaultInviteCodeSort = InviteCodeSort.Recent;

        public static class Routes {
            public static class Base {
                public const string Prefix = "com.atproto";
                public const string Server = $"{Prefix}.server";
            }

            public const string ServerCreateSession = $"{Base.Server}.createSession";
            public const string ServerRefreshSession = $"{Base.Server}.refreshSession";
        }
    }
}