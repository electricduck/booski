using Booski.Lib.Common;
using Booski.Lib.Internal.XrpcBase.Responses;
using Booski.Lib.Xrpc;

namespace Booski.Lib.Internal.XrpcBase {
    public class XrpcBaseCommand : _ {
        private readonly AtProto _atProto;

        public XrpcBaseCommand(
            AtProto atProto
        ) {
            _atProto = atProto;
        }

        public async Task<AtProtoApiResponse<GetHealthResponse>> GetHealth()
        {
            return await _atProto.GetJson<GetHealthResponse>("_health");
        }
    }
}