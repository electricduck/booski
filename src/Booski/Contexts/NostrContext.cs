using Booski.Common;
using Nostr.Client.Client;
using Nostr.Client.Communicator;

namespace Booski.Contexts;

public interface INostrContext
{
    NostrWebsocketClient? Client { get; set; }
    bool IsConnected { get; set; }
    NostrState? State { get; set; }

    Task CreateClient(string privateKey, string[] relays);
    void ResetClient();
}

// TODO: Can we use our HttpClient?
internal sealed class NostrContext : INostrContext
{
    public NostrWebsocketClient? Client { get; set; }
    public bool IsConnected { get; set; }
    public NostrState? State { get; set; }

    private NostrWebsocketCommunicator? Communicator { get; set; }

    public async Task CreateClient(
        string privateKey,
        string[] relays
    )
    {
        Uri relay = new Uri(relays[0]); // TODO: Support multiple relays
        State = new NostrState();

        try
        {
            Communicator = new NostrWebsocketCommunicator(relay);
            Client = new NostrWebsocketClient(Communicator, null);

            await Communicator.Start();

            if(Communicator.IsStarted)
            {
                State.PublicKey = "(?)"; // TODO: Figure out how to get Public Key

                IsConnected = true;
            }
            else
                IsConnected = false;
        }
        catch
        {
            ResetClient();
        }
    }

    public void ResetClient()
    {
        if(Client != null)
            Client.Dispose();
        if(Communicator != null)
            Communicator.Dispose();

        Client = null;
        Communicator = null;
        IsConnected = false;
        State = null;
    }
}