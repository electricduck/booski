using CommandLine;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Booski.Common;
using Booski.Contexts;
using Booski.Helpers;
using Booski.Lib;

namespace Booski;

public class Program
{
    public static async Task Main(string[] args)
    {
        try
        {
            HostApplicationBuilder builder = Host.CreateApplicationBuilder();

            builder.Services.AddBooskiLib();
            builder.Services.AddSingleton<IApp, App>();
            builder.Services.AddSingleton<IBskyContext, BskyContext>();
            builder.Services.AddSingleton<IBskyHelpers, BskyHelpers>();
            builder.Services.AddSingleton<IFileCacheContext, FileCacheContext>();
            builder.Services.AddSingleton<IHttpContext, HttpContext>();
            builder.Services.AddSingleton<IMastodonContext, MastodonContext>();
            builder.Services.AddSingleton<IMastodonHelpers, MastodonHelpers>();
            builder.Services.AddSingleton<IPostHelpers, PostHelpers>();
            builder.Services.AddSingleton<ITelegramContext, TelegramContext>();
            builder.Services.AddSingleton<ITelegramHelpers, TelegramHelpers>();
            builder.Services.AddSingleton<IXContext, XContext>();
            builder.Services.AddSingleton<IXHelpers, XHelpers>();

            using IHost host = builder.Build();

            var _app = host.Services.GetRequiredService<IApp>();

            _app.Options = new Options();

            await Parser.Default
                .ParseArguments<Options>(args)
                .WithParsedAsync(_app.Run);
        }
        catch (Exception e)
        {
            Say.Error(e);
            Environment.Exit(1);
        }
    }
}