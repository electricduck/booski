using Booski.Lib.Lexicon;
using Mastonet.Entities;

namespace Booski.Common;

public class MastodonState
{
    public Account Account { get; set; }
    public string Domain { get; set; }
    public InstanceV2 Instance { get; set; }
    public string InstanceSoftware { get; set; }
    public string UserId { get; set; }
    public string Username { get; set; }

    private string FallbackInstanceSoftwareString { get; } = "Mastodon (compatible)";

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

            InstanceSoftware = Instance.SourceUrl switch {
                "https://github.com/glitch-soc/mastodon" => "Glitch",
                "https://github.com/superseriousbusiness/gotosocial" => "GoToSocial",
                "https://github.com/mastodon/mastodon" => "Mastodon",
                "https://github.com/pixelfed/pixelfed" => "Pixelfed",
                _ => FallbackInstanceSoftwareString
            };

            if(InstanceSoftware == FallbackInstanceSoftwareString && Instance.Version.Contains("+"))
            {
                var versionFork = Instance.Version.Split("+")[1];

                if(versionFork.StartsWith("hometown"))
                    InstanceSoftware = "Hometown";
                    
            }
        }
    }
}