using Booski.Common;
using Booski.Contexts;
using Nostr.Client.Keys;
using Nostr.Client.Messages;
using Nostr.Client.Requests;

namespace Booski.Helpers;

public interface INostrHelpers
{
    Task PostToNostr(
        Post post,
        Embed? embed
    );
}

internal sealed class NostrHelpers : INostrHelpers
{
    private INostrContext _nostrContext;

    public NostrHelpers(
        INostrContext nostrContext
    ) {
        _nostrContext = nostrContext;
    }

    public async Task PostToNostr(
        Post post,
        Embed? embed
    )
    {
        var nostrPrv = NostrPrivateKey.FromBech32("...");

            var ev = new NostrEvent
            {
                CreatedAt = DateTime.UtcNow,
                Content = $"Hello ({DateTime.UtcNow})",
                Kind = NostrKind.ShortTextNote,

            };

            var signed = ev.Sign(nostrPrv);
            _nostrContext.Client.Send(new NostrEventRequest(signed));

            Console.WriteLine(signed.Id);
            Console.WriteLine(signed.Sig);
            Console.WriteLine(signed.Pubkey);
    }
}