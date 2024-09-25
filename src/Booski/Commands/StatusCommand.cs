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

        if (allPosts != null)
        {
            posts = allPosts
                .Where(p => p.Deleted == false)
                .Where(p => p.Ignored == Enums.IgnoredReason.None)
                .Where(
                    p =>
                        p.Mastodon_StatusId != null ||
                        p.Telegram_MessageId != null ||
                        p.X_PostId != null
                )
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

        if (pid != null)
        {
            Say.Info($"Running ({pid})", outputBody);
        }
        else
        {
            Say.Warning($"Not Running", outputBody);
            Program.Exit(true);
        }

        if(!o.NoLog)
        {
            Say.Separate();

            if(File.Exists(Program.PidLogPath))
            {
                // BUG: Performance issues if the logfile is huge
                var log = File.ReadLines(Program.PidLogPath).Reverse().Take(o.LogLines).ToList();
                foreach(var line in log)
                    Console.WriteLine(line);
            }
            else
            {
                Say.Warning("No log output");
            }
        }
    }
}
