using Booski.Lib.Common;
using Booski.Lib.Internal.AppBsky.Requests;
using Booski.Lib.Internal.AppBsky.Responses;
using Booski.Lib.Lexicon;
using AppBskyConstants = Booski.Lib.Internal.AppBsky.Common.Constants;

namespace Booski.Lib.Internal.AppBsky.Commands {
    public class ActorCommand : App.Bsky.Actor {
        private readonly AtProto _atProto;

        public ActorCommand(
            AtProto atProto
        ) {
            _atProto = atProto;
        }

        public async Task<AtProtoApiResponse<GetProfileResponse>> GetProfile(string actor)
        {
            List<QueryParam> getProfileParams = new List<QueryParam> {
                new QueryParam("actor", actor)
            };

            return await _atProto.GetJson<GetProfileResponse>(
                lexicon: AppBskyConstants.Routes.ActorGetProfile,
                queries: getProfileParams
            );
        }

        public async Task<AtProtoApiResponse<GetProfilesResponse>> GetProfiles(string[] actors)
        {
            List<QueryParam> getProfileParams = new List<QueryParam> {
                new QueryParam("actors", actors)
            };

            return await _atProto.GetJson<GetProfilesResponse>(
                lexicon: AppBskyConstants.Routes.ActorGetProfiles,
                queries: getProfileParams
            );
        }

        public async Task<AtProtoApiResponse<GetPreferencesResponse>> GetPreferences()
        {
            return await _atProto.GetJson<GetPreferencesResponse>(
                lexicon: AppBskyConstants.Routes.ActorGetPreferences
            );
        }

        public async Task<AtProtoApiResponse<GetSuggestionsResponse>> GetSuggestions(int limit = AppBskyConstants.Defaults.ResultsLimit, string cursor = "")
        {
            List<QueryParam> getSuggestionsParams = new List<QueryParam> {
                new QueryParam("cursor", [cursor]),
                new QueryParam("limit", [limit.ToString()])
            };

            return await _atProto.GetJson<GetSuggestionsResponse>(
                lexicon: AppBskyConstants.Routes.ActorGetSuggestions,
                queries: getSuggestionsParams
            );
        }

        public async Task<AtProtoApiResponse<PutPreferencesResponse>> PutPreferences(List<Polymorph> preferences)
        {
            PutPreferencesRequest putPreferencesRequest = new PutPreferencesRequest {
                Preferences = preferences
            };

            return await _atProto.PostJson<PutPreferencesResponse, PutPreferencesRequest>(
                lexicon: AppBskyConstants.Routes.ActorPutPreferences,
                body: putPreferencesRequest
            );
        }

        public async Task<AtProtoApiResponse<SearchActorsResponse>> SearchActors(string query = "", int limit = 10, string cursor = "")
        {
            List<QueryParam> searchActorsParams = new List<QueryParam> {
                new QueryParam("cursor", [cursor]),
                new QueryParam("limit", [limit.ToString()]),
                new QueryParam("q", [query])
            };

            return await _atProto.GetJson<SearchActorsResponse>(
                lexicon: AppBskyConstants.Routes.ActorSearchActors,
                queries: searchActorsParams
            );
        }

        public async Task<AtProtoApiResponse<SearchActorsTypeaheadResponse>> SearchActorsTypeahead(string query = "", int limit = 10)
        {
            List<QueryParam> searchActorsTypeaheadParams = new List<QueryParam> {
                new QueryParam("limit", [limit.ToString()]),
                new QueryParam("q", [query])
            };

            return await _atProto.GetJson<SearchActorsTypeaheadResponse>(
                lexicon: AppBskyConstants.Routes.ActorSearchActorsTypeahead,
                queries: searchActorsTypeaheadParams
            );
        }
    }
}