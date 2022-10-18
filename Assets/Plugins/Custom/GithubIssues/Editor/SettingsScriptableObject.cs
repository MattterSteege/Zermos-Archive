using Octokit;
using UnityEngine;

namespace GithubIssues
{
    //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
    public class SettingsScriptableObject : ScriptableObject
    {
        public string repoURL;
        public string token;
        [HideInInspector] public string username;
        [HideInInspector] public string repoName;
        public GitHubClient client;
    }
}