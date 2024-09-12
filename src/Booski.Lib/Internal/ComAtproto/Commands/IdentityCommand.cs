using Booski.Lib.Common;
using Booski.Lib.Internal.ComAtproto.Responses;
using Booski.Lib.Lexicon;

namespace Booski.Lib.Internal.ComAtproto.Commands {
    public class IdentityCommand : Com.Atproto.Identity
    {
        private readonly AtProto _atProto;

        public IdentityCommand(
            AtProto atProto
        ) {
            _atProto = atProto;
        }

        public async Task<AtProtoApiResponse<ResolveHandleResponse>> ResolveHandle(
            string handle
        )
        {
            List<QueryParam> resolveHandleParams = new List<QueryParam> {
                new QueryParam("handle", handle)
            };

            return await _atProto.GetJson<ResolveHandleResponse>(
                lexicon: "com.atproto.identity.resolveHandle",
                queries: resolveHandleParams
            );
        }
    }
}