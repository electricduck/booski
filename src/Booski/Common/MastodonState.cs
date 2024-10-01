using Booski.Enums;
using Mastonet.Entities;

namespace Booski.Common;

public class MastodonState
{
    public Account Account { get; set; }
    public string Domain { get; set; }
    public string FallbackInstanceSoftwareString { get; set; } = "Mastodon (compatible)";
    public InstanceV2 Instance { get; set; }
    public MastodonSoftware InstanceSoftware { get; set; }
    public bool NoRichText { get; set; } = true;
    public string UserId { get; set; }
    public string Username { get; set; }

    public string GetInstanceSoftwareString()
    {
        return InstanceSoftware switch {
            MastodonSoftware.Compatible => FallbackInstanceSoftwareString,
            MastodonSoftware.GoToSocial => "GoToSocial",
            MastodonSoftware.Mastodon => "Mastodon",
            MastodonSoftware.MastodonGlitch => "Glitch (Mastodon)",
            MastodonSoftware.MastodonHometown => "Hometown (Mastodon)",
            MastodonSoftware.Pixelfed => "Pixelfed"
        };
    }

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
                "https://github.com/glitch-soc/mastodon" => MastodonSoftware.MastodonGlitch,
                "https://github.com/superseriousbusiness/gotosocial" => MastodonSoftware.GoToSocial,
                "https://github.com/mastodon/mastodon" => MastodonSoftware.Mastodon,
                "https://github.com/pixelfed/pixelfed" => MastodonSoftware.Pixelfed,
                _ => MastodonSoftware.Compatible
            };

            if(
                InstanceSoftware == MastodonSoftware.Compatible &&
                Instance.Version.Contains("+")
            )
            {
                var versionFork = Instance.Version.Split("+")[1];

                if(versionFork.StartsWith("hometown"))
                    InstanceSoftware = MastodonSoftware.MastodonHometown;
                    
            }

            if(
                InstanceSoftware == MastodonSoftware.GoToSocial ||
                InstanceSoftware == MastodonSoftware.MastodonGlitch
            )
                NoRichText = false;
        }
    }
}