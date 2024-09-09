using Telegram.Bot.Types;

namespace Booski.Common;

public class TelegramState
{
    public User Account { get; set; }
    public string Channel { get; set; }
    public long UserId { get; set; }
    public string? Username { get; set; }

    public void SetAdditionalFields()
    {
        if(Account != null)
        {
            UserId = Account.Id;

            if(!String.IsNullOrEmpty(Account.Username))
                Username = $"@{Account.Username}";
            else
                Username = "";
        }
    }
}