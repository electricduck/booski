using System.Diagnostics;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Booski.Lib.Common;
using Booski.Lib.Enums;
using Booski.Lib.Exceptions;
using Booski.Lib.Internal.ComAtproto.Requests;
using Booski.Lib.Internal.ComAtproto.Responses;
using ComAtprotoCommon = Booski.Lib.Internal.ComAtproto.Common;

namespace Booski.Lib.Services
{
    public class AtProtoService : AtProto
    {
        private readonly HttpClient _httpClient;
        private readonly BooskiLibOptions _options;

        private readonly JsonSerializerOptions _jsonSerializerOptions = new JsonSerializerOptions
        {
            AllowOutOfOrderMetadataProperties = true,
            PropertyNameCaseInsensitive = true,
            Converters = { new JsonStringEnumConverterWithAttributeSupport() }
        };

        public Session Session { get; set; } = new Session();

        public AtProtoService(
            HttpClient httpClient
        )
        {
            _httpClient = httpClient;
        }

        #region API

        public async Task<AtProtoApiResponse<TOutput>> GetJson<TOutput>(string lexicon)
        {
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, BuildEndpoint(lexicon));
            return await Invoke<TOutput>(request);
        }

        public async Task<AtProtoApiResponse<TOutput>> GetJson<TOutput>(string lexicon, List<QueryParam> queries)
        {
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, BuildEndpoint(lexicon, queries));
            return await Invoke<TOutput>(request);
        }

        public async Task<AtProtoApiResponse<TOutput>> PostBlob<TOutput>(string lexicon, byte[] data = null, string mimeType = "*/*")
        {
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, BuildEndpoint(lexicon));

            request.Content = new ByteArrayContent(data);
            request.Content.Headers.ContentType = new MediaTypeHeaderValue(mimeType);

            return await Invoke<TOutput>(request);
        }

        public async Task<AtProtoApiResponse<TOutput>> PostJson<TOutput>(string lexicon)
        {
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, BuildEndpoint(lexicon));
            return await Invoke<TOutput>(request);
        }

        public async Task<AtProtoApiResponse<TOutput>> PostJson<TOutput, TInput>(string lexicon, TInput body = default(TInput))
        {
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, BuildEndpoint(lexicon));

            request.Content = JsonContent.Create<TInput>(body);

            return await Invoke<TOutput>(request);
        }

        #endregion

        #region Session

        public void ClearSession()
        {
            Session.Authenticated = false;
            Session.Host = Constants.DefaultHost;
            Session.Tokens = new SessionTokens();
            Session.Type = SessionType.None;
        }

        public async Task CreateSession(string identifier, string password, string host = "", string authFactorToken = "")
        {
            ClearSession();
            Session.Host = ParseHost(host);

            CreateSessionRequest createSessionRequest = new CreateSessionRequest
            {
                AuthFactorToken = authFactorToken,
                Identifier = identifier,
                Password = password
            };

            try
            {
                var createSessionResponse = await PostJson<CreateSessionResponse, CreateSessionRequest>(
                    lexicon: ComAtprotoCommon.Constants.Routes.ServerCreateSession,
                    body: createSessionRequest
                );

                if (createSessionResponse.Ok)
                {
                    Session.Authenticated = true;
                    Session.Tokens = new SessionTokens
                    {
                        Access = createSessionResponse.Data.AccessJwt,
                        Refresh = createSessionResponse.Data.RefreshJwt
                    };
                    Session.Type = SessionType.BearerToken;
                }
            }
            catch
            {
                throw;
            }
        }

        public async Task CreateSessionAsAdmin(string adminUsername, string adminPassword, string host = "")
        {
            ClearSession();
            Session.Host = ParseHost(host);

            var credentialsByteArray = Encoding.ASCII.GetBytes($"{adminUsername}:{adminPassword}");
            var credentialsBase64String = Convert.ToBase64String(credentialsByteArray);

            Session.Tokens = new SessionTokens
            {
                Access = credentialsBase64String
            };
            Session.Type = SessionType.BasicAuth;

            // TODO: Check if valid admin credentials
            Session.Authenticated = true;
        }

        public async Task CreateSessionAsGuest(string host = "")
        {
            ClearSession();
            Session.Host = ParseHost(host);
        }

        public Session GetSession()
        {
            return Session;
        }

        public async Task RefreshSession()
        {
            if (Session.Type == SessionType.BearerToken)
            {
                Session.Tokens = new SessionTokens
                {
                    Access = Session.Tokens.Refresh,
                    Refresh = String.Empty
                };

                var refreshSessionResponse = await PostJson<RefreshSessionResponse>(
                    lexicon: ComAtprotoCommon.Constants.Routes.ServerRefreshSession
                );

                if (refreshSessionResponse.Ok)
                {
                    Session.Authenticated = true;
                    Session.Tokens = new SessionTokens
                    {
                        Access = refreshSessionResponse.Data.AccessJwt,
                        Refresh = refreshSessionResponse.Data.RefreshJwt
                    };
                }
                else
                {
                    // TODO: Throw error
                    Session.Authenticated = false;
                }
            }
        }

        public async Task RefreshSession(string accessJwt = "", string refreshJwt = "", string host = "")
        {
            Session.Host = !String.IsNullOrEmpty(host) ? ParseHost(host) : Session.Host;
            Session.Tokens = new SessionTokens
            {
                Access = !String.IsNullOrEmpty(accessJwt) ? accessJwt : Session.Tokens.Access,
                Refresh = !String.IsNullOrEmpty(refreshJwt) ? refreshJwt : Session.Tokens.Refresh
            };
            Session.Type = SessionType.BearerToken;

            await RefreshSession();
        }

        public async void TrackSession()
        {
            /*await Task.Run(() => {
                while(true == true)
                {
                    Console.WriteLine("yo");
                    System.Threading.Thread.Sleep(1);
                }
            });*/
        }

        #endregion

        private async Task<AtProtoApiResponse<T>> Invoke<T>(
            HttpRequestMessage request
        )
        {
            AtProtoApiResponse<T> response = new AtProtoApiResponse<T>();
            HttpResponseMessage httpResponse;
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            if (Session.Authenticated)
            {
                request.Headers.Authorization = GetAuthenticationHeader();
            }

            httpResponse = await _httpClient.SendAsync(request);

            if (httpResponse != null)
            {
                response.Ok = httpResponse.IsSuccessStatusCode;
                response.StatusCode = httpResponse.StatusCode;

                if (httpResponse.Content != null)
                {
                    response.Type = GetHttpContentType(httpResponse);
                    response.RawData = await httpResponse.Content.ReadAsStringAsync();

                    if (response.Ok)
                    {
                        switch (response.Type)
                        {
                            case HttpContentType.Json:
                                try
                                {
                                    response.Data = await httpResponse.Content.ReadFromJsonAsync<T>(options: _jsonSerializerOptions);
                                }
                                catch (Exception e)
                                {
                                    if (!e.Message.StartsWith("The metadata property is either not supported by the type or is not the first property in the deserialized JSON object"))
                                    {
                                        throw;
                                    }
                                }
                                break;
                        }
                    }
                    else
                    {
                        if (response.Type == HttpContentType.Json)
                        {
                            await HandleError(httpResponse);
                        }
                    }
                }
            }

            stopwatch.Stop();
            response.TimeTaken = stopwatch.Elapsed;

            return response;
        }

        private string BuildEndpoint(
            string lexicon,
            List<QueryParam> queries = null
        )
        {
            string path = $"{lexicon}";
            string host = !String.IsNullOrEmpty(Session.Host) ? Session.Host : Constants.DefaultHost;

            if (queries != null)
            {
                path += "?";

                foreach (var query in queries)
                {
                    path += $"{query.QueryParamString}";
                }

                path = path.TrimEnd('&');
            }

            path = $"{host}/xrpc/{path}";
            return path;
        }

        private AuthenticationHeaderValue GetAuthenticationHeader()
        {
            AuthenticationHeaderValue authenticationHeader = null;

            if (Session.Authenticated)
            {
                switch (Session.Type)
                {
                    case SessionType.BasicAuth:
                        authenticationHeader = new AuthenticationHeaderValue("Basic", Session.Tokens.Access);
                        break;
                    case SessionType.BearerToken:
                        authenticationHeader = new AuthenticationHeaderValue("Bearer", Session.Tokens.Access);
                        break;
                }
            }

            return authenticationHeader;
        }

        private HttpContentType GetHttpContentType(HttpResponseMessage httpResponse)
        {
            HttpContentType httpContentType = HttpContentType.Unknown;
            string contentTypeString = "";

            if (httpResponse.Content != null && httpResponse.Content.Headers != null)
            {
                try
                {
                    var contentTypeHeader = httpResponse.Content.Headers.GetValues("Content-Type");

                    if (contentTypeHeader != null)
                    {
                        contentTypeString = contentTypeHeader.FirstOrDefault();
                        contentTypeString = contentTypeString.Split()[0].Trim(';');
                    }
                }
                catch
                {

                }

                switch (contentTypeString)
                {
                    case "application/json":
                        httpContentType = HttpContentType.Json;
                        break;
                }
            }

            return httpContentType;
        }

        private async Task HandleError(HttpResponseMessage httpResponse)
        {
            AtProtoApiErrorResponse errorResponse = new AtProtoApiErrorResponse();
            string errorMessage;

            if (httpResponse.Content != null)
            {
                if (GetHttpContentType(httpResponse) == HttpContentType.Json)
                {
                    errorResponse = await httpResponse.Content.ReadFromJsonAsync<AtProtoApiErrorResponse>(options: _jsonSerializerOptions);
                }
            }

            errorMessage = $"({httpResponse.StatusCode}) {errorResponse.Message}";

            switch (errorResponse.Error)
            {
                case "AuthenticationRequired":
                    throw new AtProtoAuthenticationRequiredException(errorMessage);
                case "ExpiredToken":
                    throw new AtProtoExpiredTokenException(errorMessage);
                case "InvalidRequest":
                    throw new AtProtoInvalidRequestException(errorMessage);
                case "InvalidToken":
                    throw new AtProtoInvalidTokenException(errorMessage);
                default:
                    throw new Exception(errorMessage);
            }
        }

        private string ParseHost(string host)
        {
            if (String.IsNullOrEmpty(host))
            {
                host = Constants.DefaultHost;
            }

            if (!host.StartsWith("http"))
            {
                host = $"https://{host}";
            }

            return host;
        }
    }
}