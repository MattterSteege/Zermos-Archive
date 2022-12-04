using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;

public class UpdateSystem : MonoBehaviour
{
    public enum fetchType
    {
        version,
        url,
        release_notes
    }
    
    public fetchType _fetchType;
    
    [SerializeField, Tooltip("Vx.x.x")] public string CurrentVersion;

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
        int shouldUpdate = CompareVersionStrings(CurrentVersion, GetLatestVersion());
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

    public string GetLatestVersion(fetchType type = fetchType.version)
    {
        UnityWebRequest request = UnityWebRequest.Get("https://api.github.com/repos/MJTSgamer/Zermos/releases");
        request.SetRequestHeader("Accept", "application/vnd.github+json");
        request.SetRequestHeader("Authorization",
            "Bearer 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");
        request.SendWebRequest();

        while (!request.isDone)
        {
        }

        var UpdateResponse = JsonConvert.DeserializeObject<List<UpdateResponse>>(request.downloadHandler.text);

        if (type == fetchType.url)
        {
            return UpdateResponse[0].assets[0].url;
        }
        else if (type == fetchType.release_notes)
        {
            return UpdateResponse[0].body;
        }
        else
        {
            return UpdateResponse[0].tag_name;
        }

    }

    public void DownloadLatestVersion()
    {
        string url = GetLatestVersion(fetchType.url);

        UnityWebRequest request = UnityWebRequest.Get(url);
        request.SetRequestHeader("Accept", "application/octet-stream");
        request.SetRequestHeader("Content-Type", "application/vnd.android.package-archive");
        request.SetRequestHeader("Authorization",
            "Bearer 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");

        request.SendWebRequest();

        while (!request.isDone)
        {
        }

        byte[] results = request.downloadHandler.data;

#if UNITY_EDITOR
        string path = Path.Combine(Application.persistentDataPath, "..", "..", "..", "..", "Downloads", "Zermos.apk");
#elif UNITY_ANDROID && !UNITY_EDITOR
        string path = Path.Combine(Application.persistentDataPath, "..", "..", "..", "..", "Documents", "Zermos.apk");
#endif
        bool success = SaveFile(path, results);
        
        if (success)
        {
            Debug.Log("Open 'Mijn Bestanden' > 'Interne opslag' > 'Documents' > 'Zermos.apk' to install the update");
        }
    }

    private bool SaveFile(string path, byte[] results)
    {
        try
        {
            File.WriteAllBytes(path, results);
            return true;
        }
        catch (IOException)
        {
            File.Delete(path);
            File.WriteAllBytes(path, results);
            return false;
        }
        catch (UnauthorizedAccessException)
        {
            Debug.Log("Downloaden mislukt, kijk of je de laatste versie wel verwijderd hebt, zo niet verwijder het dan! ('Mijn Bestanden' > 'Interne opslag' > 'Documents' > 'Zermos.apk')");
            return false;
        }
    }

    /// <summary>
    /// Compare two version strings, e.g.  "3.2.1.0.b40" and "3.10.1.a".
    /// V1 and V2 can have different number of components.
    /// Components must be delimited by dot.
    /// </summary>
    /// <remarks>
    /// This doesn't do any null/empty checks so please don't pass dumb parameters
    /// </remarks>
    /// <param name="v1"></param>
    /// <param name="v2"></param>
    /// <returns>
    /// -1 if v1 is lower version number than v2,
    /// 0 if v1 == v2,
    /// 1 if v1 is higher version number than v2,
    /// -1000 if we couldn't figure it out (something went wrong)
    /// </returns>
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
#endif
}