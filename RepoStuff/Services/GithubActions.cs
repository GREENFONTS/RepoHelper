using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Octokit;
using System.Net.Http;

namespace RepoStuff.Services
{
    internal class GithubActions : GithubActionInterfaces

    {
        private readonly GitHubClient _client;
        public GithubActions(GitHubClient client)
        {
            _client = client;
        }

        public async Task<List<Repository>> getRepositories()
        {           
            try {
                var user = await _client.User.Current();
                var res = await _client.Repository.GetAllForCurrent();
                List<Repository> userRepos = new List<Repository>(res);

                return userRepos;

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
#pragma warning disable CS8603 // Possible null reference return.
                return null;
#pragma warning restore CS8603 // Possible null reference return.
            }  
        }

        public string createRepository(string repoName)
        {
            try
            {
                var repository = new NewRepository(repoName)
                {
                    AutoInit = false,
                    Description = "",
                    LicenseTemplate = "mit",
                    Private = false
                };
                var context = _client.Repository.Create(repository);
                var Name = context.Result.Name;
       
#pragma warning disable CS8603 // Possible null reference return.
                return $"Repo with Name {Name} created successfully";

            }
            catch (AggregateException e)
            {
               return $"E: For some reason, the repository {repoName}  can't be created. It may already exist. {e.Message}";
            }
        }

        public async Task<string> getUserUrl()
        {
            var user = await _client.User.Current();
            var link = user.HtmlUrl;
            
            return link;
        }

        public async Task<string> deleteRepository(string repoName)
        {
            long repoId = await GetRepoId(repoName);
            if(repoId != 0)
            {
                await _client.Repository.Delete(repoId);
                return $"Repository with Name {repoName} deleted successfully";
            }
            else
            {
               return $"E: For some reason, the repository {repoName}  can't be deleted. It may not be existing.";
            }
            
        }

        public async Task<string> getRepositoryCloneUrl(string repoName)
        {
            var repos = await getRepositories();
            string cloneUrl = "";
            foreach (var repo in repos)
            {
                if (repo.Name.ToLower() == repoName.ToLower()) cloneUrl = repo.CloneUrl;
            }
            if (cloneUrl != "")
            {
                return cloneUrl;
            }
            else
            {
                return $"E: For some reason, the repository {repoName}  clone url can't be found. The Repository may not be existing.";
            }
            
        }

        public async Task<Issue> createIssue(string repoName, string title, string description)
        {
            var issueMessage = new NewIssue(title);
            issueMessage.Body = description;
            var user = await _client.User.Current();
            try
            {
                string[] be = user.Url.Split("users/");
                string Owner = be[1];

                var issue = await _client.Issue.Create(Owner, repoName, issueMessage);
                
                return issue;
            }
            catch (NotFoundException e)
            {
                if(e.Message == "Not Found")
                {
                    Console.WriteLine($"E: For some reason, the repository {repoName}  clone url can't be found. The Repository may not be existing.");
                }
                Console.WriteLine("Owner " + e.Message);
                return null;
            }
        }

        public async Task<long> GetRepoId(string repoName)
        {
            var repos = await getRepositories();
            long repoId = 0;
            foreach (var repo in repos)
            {
                if (repo.Name.ToLower() == repoName.ToLower()) repoId = repo.Id;
            }
            return repoId;
        }

    }

   
}
