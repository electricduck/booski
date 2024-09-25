using LinqToTwitter;

namespace Booski.Common;

public class XState
{
    public Account? Account { get; set; }
    public string? Username { get; set; }

    public void SetAdditionalFields()
    {
        if(Account != null && Account.User != null)
        {
            Username = $"@{Account.User.ScreenNameResponse}";
        }
    }
}