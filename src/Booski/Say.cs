using System.Runtime.InteropServices;
using System.Text;
using Booski.Utilities;

namespace Booski;

public class Say
{
    static bool IsLastSaySeparate { get; set; }

    public static void Custom(string message, string reason = "", string emoji = "", bool separate = false)
    {
        ConsoleMessage(message, emoji, reason, separate);
    }

    public static void Debug(string message, string reason = "")
    {
        bool say = false;

#if DEBUG
        say = true;
#else
        say = EnvUtilities.GetEnvBool("Debug");
#endif

        if(say)
            ConsoleMessage(message, "⚙️", reason);
    }

    public static void Error(string message, string reason = "")
    {
        ConsoleMessage($"Error: {message}", "🛑", reason);
    }

    public static void Error(Exception e)
    {
        Error(e.Message);
    }

    public static void Info(string message, string reason = "")
    {
        ConsoleMessage(message, "ℹ️", reason);
    }

    public static void Separate(int length = 80, char separator = '-')
    {
        if(!IsLastSaySeparate && !Program.NoSay)
        {
            Console.WriteLine(new String(separator, length));
            IsLastSaySeparate = true;
        }
    }

    public static void Success(string message)
    {
        ConsoleMessage(message, "✅");
    }

    public static void Warning(string message, string reason = "", bool separate = false)
    {
        ConsoleMessage(message, "⚠️", reason, separate);
    }

    static void ConsoleMessage(
        string message,
        string emoji = "",
        string reason = "",
        bool separate = false
    )
    {
        Console.OutputEncoding = Encoding.UTF8;

        if(!String.IsNullOrEmpty(emoji))
        {
            // BUG: If you're SSH'd into Windows from macOS/Linux,
            //      this won't trigger and the emoji padding will still be borked
            if(!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                var emojiByteLength = Encoding.UTF8.GetBytes(emoji).Length;
                if(emojiByteLength > 4)
                {
                    message = $" {message}";
                }
            }

            message = $"{emoji} {message}";
        }

        if(!String.IsNullOrEmpty(reason))
        {
            string paddedReason = "";

            using (StringReader reader = new StringReader(reason))
            {
                string line = string.Empty;
                do
                {
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
                    line = reader.ReadLine();
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
                    if (line != null)
                    {
                        paddedReason += Environment.NewLine;
                        if(!String.IsNullOrEmpty(emoji))
                            paddedReason += $"   ";
                        paddedReason += line;
                    }

                } while (line != null);
            }

            message += paddedReason;
        }
        
        if(!Program.NoSay)
        {
            if(separate)
                Separate();
            Console.WriteLine(message);
            IsLastSaySeparate = false;
            if(separate)
                Separate();
        }
        else
        {
            int? pid = Pid.GetPid();
            if(pid != null && pid > 0)
                LogMessage(message);
        }
    }

    static void LogMessage(string message)
    {
        if(File.Exists(Program.PidLogPath))
            File.AppendAllText(Program.PidLogPath, $"{message}\n");
    }
}