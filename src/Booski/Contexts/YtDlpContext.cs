using System.Diagnostics;

namespace Booski.Contexts;

public interface IYtDlpContext
{
    (string, int?) DownloadVideo(Uri uri, string outputPath);
    bool DoesYtDlpExist();
}

internal sealed class YtDlpContext : IYtDlpContext
{
    private IHttpContext _httpContext;

    public YtDlpContext(
        IHttpContext httpContext
    )
    {
        _httpContext = httpContext;
    }

    public (string, int?) DownloadVideo(Uri uri, string outputPath)
    {
        int? exitCode = null;

        if(DoesYtDlpExist())
        {
            Process process = RunYtDlp("--output", outputPath, uri.ToString());
            if(process != null)
                exitCode = process.ExitCode;
        }

        return (outputPath, exitCode);
    }

    public bool DoesYtDlpExist()
    {
        var process = RunYtDlp("--version");
        var success = (process != null && process.ExitCode == 0) ? true : false;
        return success;
    }

    Process? RunYtDlp(params string[] args)
    {
        try
        {
            var processInfo = new ProcessStartInfo();

            string argumentsString = "";

            foreach (var arg in args)
            {
                argumentsString += $"{arg} ";
            }

            processInfo.Arguments = argumentsString;
            processInfo.FileName = Program.YtDlpPath;
            processInfo.RedirectStandardError = true;
            processInfo.RedirectStandardOutput = true;
            processInfo.UseShellExecute = false;

            Process process = Process.Start(processInfo);
            process.WaitForExit();

            return process;
        }
        catch
        {
            return null;
        }
    }
}