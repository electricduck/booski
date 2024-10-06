using Booski.Lib.Common;
using Booski.Lib.Internal.ComAtproto.Requests;
using Booski.Lib.Internal.ComAtproto.Responses;
using Booski.Lib.Xrpc;

namespace Booski.Lib.Internal.ComAtproto.Commands {
    public class SyncCommand : Com.Atproto.Sync
    {
        private readonly AtProto _atProto;

        public SyncCommand(
            AtProto atProto
        ) {
            _atProto = atProto;
        }

        public async Task<AtProtoApiResponse<GetLatestCommitResponse>> GetLatestCommit(string did)
        {
            List<QueryParam> getLatestCommitParams = new List<QueryParam> {
                new QueryParam("did", did)
            };

            return await _atProto.GetJson<GetLatestCommitResponse>(
                lexicon: "com.atproto.sync.getLatestCommit",
                queries: getLatestCommitParams
            );
        }
    }
}