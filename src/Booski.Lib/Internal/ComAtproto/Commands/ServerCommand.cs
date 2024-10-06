using Booski.Lib.Common;
using Booski.Lib.Internal.ComAtproto.Requests;
using Booski.Lib.Internal.ComAtproto.Responses;
using Booski.Lib.Xrpc;

namespace Booski.Lib.Internal.ComAtproto.Commands {
    public class ServerCommand : Com.Atproto.Server
    {
        private readonly AtProto _atProto;

        public ServerCommand(
            AtProto atProto
        ) {
            _atProto = atProto;
        }

        public async Task<AtProtoApiResponse<CheckAccountStatusResponse>> CheckAccountStatus()
        {
            return await _atProto.GetJson<CheckAccountStatusResponse>(
                lexicon: "com.atproto.server.checkAccountStatus"
            );
        }

        public async Task<AtProtoApiResponse<CreateInviteCodeResponse>> CreateInviteCode(int useCount, string forAccount = "")
        {
            CreateInviteCodeRequest createInviteCodeRequest = new CreateInviteCodeRequest {
                UseCount = useCount
            };

            if(!String.IsNullOrEmpty(forAccount))
                createInviteCodeRequest.ForAccount = forAccount;

            return await _atProto.PostJson<CreateInviteCodeResponse, CreateInviteCodeRequest>(
                lexicon: "com.atproto.server.createInviteCode",
                body: createInviteCodeRequest
            );
        }

        public async Task<AtProtoApiResponse<CreateSessionResponse>> CreateSession(string identifier, string password, string? authFactorToken = "")
        {
            CreateSessionRequest createSessionRequest = new CreateSessionRequest {
                AuthFactorToken = authFactorToken,
                Identifier = identifier,
                Password = password,
            };

            return await _atProto.PostJson<CreateSessionResponse, CreateSessionRequest>(
                lexicon: "com.atproto.server.createSession",
                body: createSessionRequest
            );
        }

        public async Task<AtProtoApiResponse<DescribeServerResponse>> DescribeServer()
        {
            return await _atProto.GetJson<DescribeServerResponse>(
                lexicon: "com.atproto.server.describeServer"
            );
        }

        public async Task<AtProtoApiResponse<GetSessionResponse>> GetSession()
        {
            return await _atProto.GetJson<GetSessionResponse>(
                lexicon: "com.atproto.server.getSession"
            );
        }

        public async Task<AtProtoApiResponse<RefreshSessionResponse>> RefreshSession()
        {
            return await _atProto.PostJson<RefreshSessionResponse>(
                lexicon: "com.atproto.server.refreshSession"
            );
        }
    }
}