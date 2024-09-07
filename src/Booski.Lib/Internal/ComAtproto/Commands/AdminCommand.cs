using Booski.Lib.Common;
using Booski.Lib.Internal.ComAtproto.Common;
using Booski.Lib.Internal.ComAtproto.Enums;
using Booski.Lib.Internal.ComAtproto.Responses;
using Booski.Lib.Lexicon;
using Booski.Lib.Utilities;
using ComAtprotoConstants = Booski.Lib.Internal.ComAtproto.Common.Constants;

namespace Booski.Lib.Internal.ComAtproto.Commands {
    public class AdminCommand : Com.Atproto.Admin
    {
        private readonly AtProto _atProto;

        public AdminCommand(
            AtProto atProto
        ) {
            _atProto = atProto;
        }

        public async Task<AtProtoApiResponse<GetInviteCodesResponse>> GetInviteCodes(
            int limit = ComAtprotoConstants.DefaultInviteCodeLimit,
            InviteCodeSort sort = ComAtprotoConstants.DefaultInviteCodeSort
        )
        {
            string sortString = EnumUtilities.GetEnumMemberValue<InviteCodeSort>(sort);

            List<QueryParam> getInviteCodesParams = new List<QueryParam> {
                new QueryParam("limit", limit.ToString()),
                new QueryParam("sort", sortString)
            };

            return await _atProto.GetJson<GetInviteCodesResponse>(
                lexicon: "com.atproto.admin.getInviteCodes",
                queries: getInviteCodesParams
            );
        }
    }
}