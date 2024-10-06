using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Booski.Lib.Services;
using Booski.Lib.Internal.AppBsky.Commands;
using Booski.Lib.Internal.ComAtproto.Commands;
using Booski.Lib.Internal.XrpcBase;
using AppBskyServices = Booski.Lib.Services.AppBsky;

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
            services.AddScoped<Xrpc.App.Bsky.Actor, AppBskyServices.ActorService>();
            services.AddScoped<Xrpc.App.Bsky.Feed, FeedCommand>();
            services.AddScoped<Xrpc.Com.Atproto.Admin, AdminCommand>();
            services.AddScoped<Xrpc.Com.Atproto.Identity, IdentityCommand>();
            services.AddScoped<Xrpc.Com.Atproto.Repo, RepoCommand>();
            services.AddScoped<Xrpc.Com.Atproto.Server, ServerCommand>();
            services.AddScoped<Xrpc.Com.Atproto.Sync, SyncCommand>();
            services.AddScoped<Xrpc._, XrpcBaseCommand>();
        
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