using Booski.Common;
using Booski.Common.Config;
using Booski.Lib;
using BskyApi = Booski.Lib.Lexicon;

namespace Booski.Contexts;

public interface IBskyContext
{
    Task CreateSession(ClientsConfig clientsConfig);
    Task CreateSession(
        string username,
        string password,
        string host
    );
    bool IsConnected { get; set; }
    BskyState? State { get; set; }
}

internal sealed class BskyContext : IBskyContext
{
    public bool IsConnected { get; set; }
    public BskyState? State { get; set; }

    private AtProto _atProto;
    private BskyApi.App.Bsky.Actor _bskyActor;

    public BskyContext(
        AtProto atProto,
        BskyApi.App.Bsky.Actor bskyActor
    )
    {
        _atProto = atProto;
        _bskyActor = bskyActor;
    }

    public async Task CreateSession(ClientsConfig clientsConfig)
    {
        if (
            clientsConfig != null &&
            clientsConfig?.Bluesky != null
        )
        {
#pragma warning disable CS8604 // Possible null reference argument.
            await CreateSession(
                clientsConfig?.Bluesky?.Username,
                clientsConfig?.Bluesky?.Password,
                clientsConfig?.Bluesky?.Host
            );
#pragma warning restore CS8604 // Possible null reference argument.
        }
    }

    public async Task CreateSession(
        string username,
        string password,
        string host
    )
    {
        State = new BskyState();

        await _atProto.CreateSession(
            username,
            password,
            host
        );

    if (_atProto.GetSession() != null)
        {
            var bskyProfileResponse = await _bskyActor.GetProfile(username);
            State.Profile = bskyProfileResponse.Data;
            State.SetAdditionalFields();
        }
    }
}