using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class InPlanLes : MonoBehaviour
{
    public string post = null;
    
    public void enrollIntoLesson()
    {
        if (post == null) return;
        
        string baseURL = "https://{school}.zportal.nl" + post;
        
        baseURL = baseURL.Replace("{school}", PlayerPrefs.GetString("zermelo-school_code"));
        
        UnityWebRequest www = UnityWebRequest.Post(baseURL, new WWWForm());
        www.SetRequestHeader("Authorization", "Bearer " + PlayerPrefs.GetString("zermelo-access_token"));
        
        www.SendWebRequest();

        while (!www.isDone) { }
        
        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(www.error);
        }
        else
        {
            Debug.Log("Form upload complete!");
            
            ViewManager.Instance.Refresh<DagRoosterView>();
            ViewManager.Instance.Show<NavBarView, DagRoosterView>();
        }
    }
}
