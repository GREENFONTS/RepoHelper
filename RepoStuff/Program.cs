using Microsoft.Win32;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Octokit;
using Octokit.Internal;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.Json;

namespace RepoStuff
{

    public class RepoMain
    {
        public static async Task Main(string[] args)
        {

            string clientId = "7884a1308e3c1e372d71";
            string scope = "repo delete_repo";
            string token = "";

#pragma warning disable CA1416 // Validate platform compatibility

            RegistryKey key = Registry.CurrentUser.CreateSubKey(@"SOFTWARE\OurSettings");
            if (key != null)
            {
#pragma warning disable CS8602 // Dereference of a possibly null reference.
                var result = key.GetValue("token");
                if (result == null)
                {
                    token = await GitDeviceFlow.GitFlow.GetAccessToken(clientId, scope);
                    key.SetValue("token", token);
                }
                else
                {
                    token = result.ToString()!;
                }
                key.Close();
            }

            var creds = new InMemoryCredentialStore(new Credentials(token));
            var gitClient = new GitHubClient(new ProductHeaderValue("Git-helper"), creds);

            var instanceClass = new Actions.Functions(gitClient);

            CallFunctions(args, instanceClass);
            Console.ReadLine();
        }


        public static void CallFunctions(string[] args, Actions.Functions instanceClass)
        {
            string action = "";
            string repoName = "";
            if (args.Length == 0)
            {
                instanceClass.help();
            }
            if (args.Length == 1)
            {
                action = args[0];
            }
            if (args.Length == 2)
            {
                action = args[0];
                repoName = args[1];
            }
            if(args.Length > 2)
            {
                instanceClass.help();
            }
            switch (action.ToLower())
            {
                case "getrepos":
                    instanceClass.GetRepos();
                    break;
                case "createrepo":
                    instanceClass.CreateRepo(repoName);
                    break;
                case "deleterepo":
                    instanceClass.DeleteRepo(repoName);
                    break;
                case "getuserurl":
                    instanceClass.GetUserUrl();
                    break;
                case "repocloneurl":
                    instanceClass.RepoCloneUrl(repoName);
                    break;
                case "createissue":
                    instanceClass.createIssue(repoName);
                    break;
                case "help":
                    instanceClass.help();
                    break;

            }
        }
    }

}

