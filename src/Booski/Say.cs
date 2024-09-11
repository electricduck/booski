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
#if DEBUG
        ConsoleMessage(message, "⚙️", reason);
#endif
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
        var emojiByteLength = Encoding.UTF8.GetBytes(emoji).Length;
        var emojiPadding = 0;

        // i don't fuckin understand unicode
        switch(emojiByteLength)
        {
            case 3:
            case 4:
                emojiPadding = 1;
                break;
            case 5:
            case 6:
                emojiPadding = 2;
                break;
        }

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