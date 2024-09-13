using Booski.Common.Options;
using Booski.Data;

namespace Booski.Commands;

public interface IStatusCommand
{
    StatusOptions Options { get; set; }

    Task Invoke(StatusOptions o);
}

internal sealed class StatusCommand : IStatusCommand
{
    public StatusOptions Options { get; set; }

    public async Task Invoke(StatusOptions o)
    {
        int? pid = Pid.GetPid();

        var allPosts = await PostLogs
            .GetAllPostLogs();

        int posts = 0;
        int postsDeleted = 0;
        int postsIgnored = 0;
        int postsTotal = 0;

        if(allPosts != null)
        {
            // BUG: This includes posts that haven't actually posted anywhere
            posts = allPosts
                .Where(p => p.Deleted == false)
                .Where(p => p.Ignored != Enums.IgnoredReason.None)
                .ToList()
                .Count();
            postsDeleted = allPosts
                .Where(p => p.Deleted == true)
                .ToList()
                .Count();
            postsIgnored = allPosts
                .Where(p => p.Ignored != Enums.IgnoredReason.None)
                .ToList()
                .Count();
            postsTotal = allPosts.Count();
        }

        string outputBody = $@"↳ Total:   {postsTotal}
↳ Posted:  {posts}
↳ Deleted: {postsDeleted}
↳ Ignored: {postsIgnored}
";

        if(pid != null)
        {
            Say.Info($"Running ({pid})", outputBody);
        }
        else
        {
            Say.Warning($"Not Running", outputBody);
            Program.Exit(true);
        }
    }
}
