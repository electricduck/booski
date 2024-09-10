using Octokit;

namespace Booski.Contexts;

public interface IGitHubContext
{
    GitHubClient? Client { get; set; }

    Task CreateClient();
    void ResetClient();
}

internal sealed class GitHubContext : IGitHubContext
{
    public GitHubClient? Client { get; set; }

    public async Task CreateClient()
    {
        Client = new GitHubClient(new ProductHeaderValue("Booski"));
    }

    public void ResetClient()
    {
        Client = null;
    }
}