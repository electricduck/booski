using Mastonet.Entities;

namespace Booski.Common;

public class MastodonState
{
    public Account Account { get; set; }
    public string Domain { get; set; }
    public InstanceV2 Instance { get; set; }
    public string UserId { get; set; }
    public string Username { get; set; }

    public void SetAdditionalFields()
    {
        if(
            Account != null &&
            Instance != null
        )
        {
            Domain = Instance.Domain;
            UserId = Account.Id;
            Username = $"@{Account.UserName}@{Instance.Domain}";
        }
    }
}