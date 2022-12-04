using UnityEngine;

namespace GithubIssues
{
    [CreateAssetMenu(fileName = "Settings", menuName = "GithubIssues/Settings", order = 1)]
    public class SettingsScriptableObject : ScriptableObject
    {
        public string repoURL;
        public string token;
    }
}