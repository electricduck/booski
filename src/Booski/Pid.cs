using System.Diagnostics;

namespace Booski;

public static class Pid
{
    public static int? CreatePid(int? pid = null)
    {
        if(pid == null)
            pid = Program.CurrentProcess.Id;

        if(!String.IsNullOrEmpty(Program.PidPath))
            File.WriteAllText(Program.PidPath, pid.ToString());
        
        if(!String.IsNullOrEmpty(Program.PidLogPath))
            File.Create(Program.PidLogPath);
        
        return pid;
    }

    public static void DeletePid()
    {
        if(
            !String.IsNullOrEmpty(Program.PidPath) &&
            File.Exists(Program.PidPath)
        )
            File.Delete(Program.PidPath);

        if(
            !String.IsNullOrEmpty(Program.PidLogPath) &&
            File.Exists(Program.PidLogPath)
        )
            File.Delete(Program.PidLogPath);
    }

    public static int? GetPid()
    {
        int? pid = null;

        if(
            !String.IsNullOrEmpty(Program.PidPath) &&
            File.Exists(Program.PidPath)
        )
        {
            using StreamReader pidFileReader = new(Program.PidPath);
            string pidFileText = pidFileReader.ReadToEnd();

            if(!String.IsNullOrEmpty(pidFileText))
            {
                pid = Convert.ToInt32(pidFileText);

                try
                {
                    Process.GetProcessById((int)pid);
                }
                catch
                {
                    pid = null;
                }
            }

            pidFileReader.Close();

            if(pid == null)
                DeletePid();
        }

        return pid;
    }
}