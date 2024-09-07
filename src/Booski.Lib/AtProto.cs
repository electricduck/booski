using Booski.Lib.Common;

namespace Booski.Lib
{
    public interface AtProto
    {
        Task<AtProtoApiResponse<TOutput>> GetJson<TOutput>(string lexicon);
        Task<AtProtoApiResponse<TOutput>> GetJson<TOutput>(string lexicon, List<QueryParam> queries);
        Task<AtProtoApiResponse<TOutput>> PostBlob<TOutput>(string lexicon, byte[] data = null, string mimeType = "*/*");
        Task<AtProtoApiResponse<TOutput>> PostJson<TOutput>(string lexicon);
        Task<AtProtoApiResponse<TOutput>> PostJson<TOutput, TInput>(string lexicon, TInput body = default(TInput));
        void ClearSession();
        Task CreateSession(string identifier, string password, string host = "", string authFactorToken = "");
        Task CreateSessionAsAdmin(string adminUsername, string adminPassword, string host = "");
        Task CreateSessionAsGuest(string host = "");
        Session GetSession();
        Task RefreshSession();
        Task RefreshSession(string accessJwt = "", string refreshJwt = "", string host = "");
        void TrackSession();
    }
}