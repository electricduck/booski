using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Booski.Lib.Services;
using Booski.Lib.Internal.AppBsky.Commands;
using Booski.Lib.Internal.ComAtproto.Commands;
using Booski.Lib.Internal.XrpcBase;

namespace Booski.Lib
{
    public static class ServiceCollectionExtensions
    {
        public static IHttpClientBuilder AddBooskiLib(
            this IServiceCollection services,
            Action<BooskiLibOptions>? configurator = null)
        {
            var options = new BooskiLibOptions();
            configurator?.Invoke(options);
            services.TryAddSingleton(options);

            services.AddScoped<AtProto, AtProtoService>();
            services.AddScoped<Lexicon.App.Bsky.Actor, ActorCommand>();
            services.AddScoped<Lexicon.App.Bsky.Feed, FeedCommand>();
            services.AddScoped<Lexicon.Com.Atproto.Admin, AdminCommand>();
            services.AddScoped<Lexicon.Com.Atproto.Repo, RepoCommand>();
            services.AddScoped<Lexicon.Com.Atproto.Server, ServerCommand>();
            services.AddScoped<Lexicon.Com.Atproto.Sync, SyncCommand>();
            services.AddScoped<Lexicon._, XrpcBaseCommand>();
        
            //services.AddScoped<Booski.LibOptions, BlueskyApi>();
            //return services.AddHttpClient(Constants.BlueskyApiClient, (_, client) =>
            //{
            //    client.DefaultRequestHeaders.Add("Accept", Constants.AcceptedMediaType);
            //    client.BaseAddress = new Uri(options.Url.TrimEnd('/'));
            //    client.DefaultRequestHeaders.Add(Constants.HeaderNames.UserAgent, options.UserAgent);
            //});

            return services.AddHttpClient("Bluesky", (_, client) => {});
        }
    }
}