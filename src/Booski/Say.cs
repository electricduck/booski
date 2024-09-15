using System.Text;

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
        var isDebug = Environment.GetEnvironmentVariable("BOOSKI_DEBUG");

        if (isDebug == "1" || isDebug == "true")
            say = true;
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
        if(!IsLastSaySeparate)
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
        var emojiPadding = 1;

        var emojiPaddingString = new String(' ', emojiPadding);

        if(!String.IsNullOrEmpty(emoji))
            message = $"{emoji}{emojiPaddingString}{message}";

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
                        if(!String.IsNullOrEmpty(emojiPaddingString))
                            paddedReason += $"{emojiPaddingString} ";
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
    }
}