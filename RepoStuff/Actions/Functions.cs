using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Octokit;
using System.Net.Http;

namespace RepoStuff.Actions
{
    public class Functions : funcInterfaces
    {
        private readonly Services.GithubActions actionService;
        public Functions(GitHubClient client)
        {
            actionService = new Services.GithubActions(client);
        }


        //GET ALL REPOS
        public async void GetRepos()
        {
            var repos = await actionService.getRepositories();
            foreach (var repo in repos)
            {
                Console.WriteLine(repo.Name);
            }
        }


        //TO CREATE A REPO
        public void CreateRepo(string repoName)
        {
            var res = actionService.createRepository(repoName);
            Console.WriteLine(res);
        }

        //TO GET USER GITHUB URL
        public async void GetUserUrl()
        {
            var res = await actionService.getUserUrl();
            Console.WriteLine(res);
        }


        //TO DELETE GITHUB REPO
        public async void DeleteRepo(string repoName)
        {
            var res = await actionService.deleteRepository(repoName);
            Console.WriteLine(res);
        }


        //TO GET CLONE URL
        public async void RepoCloneUrl(string repoName)
        {
            var res = await actionService.getRepositoryCloneUrl(repoName);
            Console.WriteLine(res);
        }


        //SET TO PRIVATE
        public async void createIssue(string repoName)
        {
            Console.Write("Enter Issue Title:");
            string title = Console.ReadLine()!;
            Console.Write("Enter Issue Description:");
            string description = Console.ReadLine()!;

            var res = await actionService.createIssue(repoName, title, description);
            if (res == null)
            {
                Console.WriteLine($"E: For some reason, the repository {repoName}  can't be created. It may already exist.");
            }
            else
            {
                Console.WriteLine($"Issue with title: {res.Title} and state: {res.State} created successfully");
            }

        }

        //TOOL HELP ROUTE
        public void help()
        {
            Console.WriteLine();
            Console.WriteLine(" This is a Git Helper CLI Tool built to allow users implement some github actions in the terminal rather than using the Github Web Interface");
            Console.WriteLine();

            Console.WriteLine(" All commands are non-case sensitive and must begin with: gitCli");
            Console.WriteLine();

            Console.WriteLine("     getRepos                     --      lists all the public repos of the user");
            Console.WriteLine("     createRepo {repoName}        --      creates a new repo for the user with the inputted repoName");
            Console.WriteLine("     delRepo {repoName}           --      deletes a public repo for the user with the inputted repoName");
            Console.WriteLine("     getUserUrl                   --      gets the user's github url");
            Console.WriteLine("     repoCloneUrl {repoName}      --      gets the user's public repo clone url for the inputted repoName");
            Console.WriteLine("     createIssue {repoName}       --      creates and issue on the public repo of the user with the inputted repoName");


        }
    }

   
}
