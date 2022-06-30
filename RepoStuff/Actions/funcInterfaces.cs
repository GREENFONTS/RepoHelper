using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Octokit;

namespace RepoStuff.Actions
{
    internal interface funcInterfaces
    {
        void GetRepos();

        void CreateRepo(string repoName);

        void GetUserUrl();

        void DeleteRepo(string repoName);

        void RepoCloneUrl(string repoName);

        void createIssue(string repoName);

        void help();

    }
}
