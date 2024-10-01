using System.Diagnostics;
using Booski.Common.Options;
using Booski.Enums;
using Booski.Helpers;

namespace Booski.Commands;

public interface IStopCommand
{
    StopOptions Options { get; set; }

    Task Invoke(StopOptions o);
}

internal sealed class StopCommand : IStopCommand
{
    public StopOptions Options { get; set; }

    private II18nHelpers _i18n;

    public StopCommand(
        II18nHelpers i18nHelpers
    )
    {
        _i18n = i18nHelpers;
    }

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

                Say.Success(
                    _i18n.GetPhrase(Phrase.Console_StopCommand_DaemonStopped)
                );
            }
            catch
            {                
                Say.Error(
                    _i18n.GetPhrase(
                        Phrase.Console_StopCommand_DaemonError,
                        pid.ToString()
                    )
                );
            }
        }
        else
        {
            Say.Error(
                _i18n.GetPhrase(Phrase.Console_StopCommand_DaemonNotRunning)
            );
        }
    }
}