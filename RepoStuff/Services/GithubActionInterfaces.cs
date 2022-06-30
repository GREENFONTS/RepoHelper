using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Octokit;

namespace RepoStuff.Services
{
    internal interface GithubActionInterfaces
    {
        Task<List<Repository>> getRepositories();

        string createRepository(string repoName);

        Task<string> getUserUrl();

        Task<string> deleteRepository(string repoName);

        Task<string> getRepositoryCloneUrl(string repoName);

        Task<Issue> createIssue(string message, string title, string description);


    }
}
