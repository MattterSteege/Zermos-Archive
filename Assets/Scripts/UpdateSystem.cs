using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;
using Debug = UnityEngine.Debug;

public class UpdateSystem : MonoBehaviour
{
    public enum fetchType
    {
        version,
        url,
        release_notes
    }
    
    [HideInInspector] public fetchType _fetchType;
    public string CurrentVersion => Application.version;

#if UNITY_ANDROID
    /// <summary>
    /// 1 - Update available
    /// 0 - Up to date             
    /// -1 - higher version        
    /// -2 - error                 
    /// </summary>
    /// <returns>if there is an update available</returns>
    [ContextMenu("Check for update")]
    public int checkForUpdates()
    {
        int shouldUpdate = 0;//CompareVersionStrings(CurrentVersion, GetLatestVersion());
        if (shouldUpdate == -1)
        {
            return 1;
        }

        if (shouldUpdate == 0)
        {
            return 0;
        }

        if (shouldUpdate == 1)
        {
            return -1;
        }

        return -2;
    }

    private static int CompareVersionStrings(string v1, string v2)
    {
        int rc = -1000;

        v1 = v1.ToLower();
        v2 = v2.ToLower();

        if (v1 == v2)
            return 0;

        string[] v1parts = v1.Split('.');
        string[] v2parts = v2.Split('.');

        for (int i = 0; i < v1parts.Length; i++)
        {
            if (v2parts.Length < i + 1)
                break; // we're done here

            string v1Token = v1parts[i];
            string v2Token = v2parts[i];

            int x;
            bool v1Numeric = int.TryParse(v1Token, out x);
            bool v2Numeric = int.TryParse(v2Token, out x);

            // handle scenario {"2" versus "20"} by prepending zeroes, e.g. it would become {"02" versus "20"}
            if (v1Numeric && v2Numeric)
            {
                while (v1Token.Length < v2Token.Length)
                    v1Token = "0" + v1Token;
                while (v2Token.Length < v1Token.Length)
                    v2Token = "0" + v2Token;
            }

            rc = String.Compare(v1Token, v2Token, StringComparison.Ordinal);
            //Console.WriteLine("v1Token=" + v1Token + " v2Token=" + v2Token + " rc=" + rc);
            if (rc != 0)
                break;
        }

        if (rc == 0)
        {
            // catch this scenario: v1="1.0.1" v2="1.0"
            if (v1parts.Length > v2parts.Length)
                rc = 1; // v1 is higher version than v2
            // catch this scenario: v1="1.0" v2="1.0.1"
            else if (v2parts.Length > v1parts.Length)
                rc = -1; // v1 is lower version than v2
        }

        if (rc == 0 || rc == -1000)
            return rc;
        else
            return rc < 0 ? -1 : 1;
    }

    #region models
    public class Asset
    {
        public string url { get; set; }
        public int id { get; set; }
        public string node_id { get; set; }
        public string name { get; set; }
        public object label { get; set; }
        public Uploader uploader { get; set; }
        public string content_type { get; set; }
        public string state { get; set; }
        public int size { get; set; }
        public int download_count { get; set; }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
        public string browser_download_url { get; set; }
    }

    public class Author
    {
        public string login { get; set; }
        public int id { get; set; }
        public string node_id { get; set; }
        public string avatar_url { get; set; }
        public string gravatar_id { get; set; }
        public string url { get; set; }
        public string html_url { get; set; }
        public string followers_url { get; set; }
        public string following_url { get; set; }
        public string gists_url { get; set; }
        public string starred_url { get; set; }
        public string subscriptions_url { get; set; }
        public string organizations_url { get; set; }
        public string repos_url { get; set; }
        public string events_url { get; set; }
        public string received_events_url { get; set; }
        public string type { get; set; }
        public bool site_admin { get; set; }
    }

    public class UpdateResponse
    {
        public string url { get; set; }
        public string assets_url { get; set; }
        public string upload_url { get; set; }
        public string html_url { get; set; }
        public int id { get; set; }
        public Author author { get; set; }
        public string node_id { get; set; }
        public string tag_name { get; set; }
        public string target_commitish { get; set; }
        public string name { get; set; }
        public bool draft { get; set; }
        public bool prerelease { get; set; }
        public DateTime created_at { get; set; }
        public DateTime published_at { get; set; }
        public List<Asset> assets { get; set; }
        public string tarball_url { get; set; }
        public string zipball_url { get; set; }
        public string body { get; set; }
    }

    public class Uploader
    {
        public string login { get; set; }
        public int id { get; set; }
        public string node_id { get; set; }
        public string avatar_url { get; set; }
        public string gravatar_id { get; set; }
        public string url { get; set; }
        public string html_url { get; set; }
        public string followers_url { get; set; }
        public string following_url { get; set; }
        public string gists_url { get; set; }
        public string starred_url { get; set; }
        public string subscriptions_url { get; set; }
        public string organizations_url { get; set; }
        public string repos_url { get; set; }
        public string events_url { get; set; }
        public string received_events_url { get; set; }
        public string type { get; set; }
        public bool site_admin { get; set; }
    }
#endregion

#endif
}