using System.Diagnostics;
using Booski.Common.Options;

namespace Booski.Commands;

public interface IStopCommand
{
    StopOptions Options { get; set; }

    Task Invoke(StopOptions o);
}

internal sealed class StopCommand : IStopCommand
{
    public StopOptions Options { get; set; }

    public async Task Invoke(StopOptions o)
    {
        int? pid = Pid.GetPid();

        if(pid != null)
        {
            try
            {
                var runningProcess = Process.GetProcessById((int)pid);
                runningProcess.Kill();
                Pid.DeletePid();
                Say.Success("Stopped daemon");
            }
            catch
            {
                Say.Error($"Unable to stop daemon ({pid})");
            }
        }
        else
        {
            Say.Error("Daemon not running");
        }
    }
}