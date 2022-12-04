using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Leermiddelen : MonoBehaviour
{
    //BROKEN AS SHIT!!!
    [ContextMenu("get Grades")]
    public void getLeermiddelen()
    {
        UnityWebRequest www = UnityWebRequest.Get("https://elo.somtoday.nl/home/leermiddelen");
        
        www.SetRequestHeader("accept", "text/html");
        www.SetRequestHeader("Cookie", "JSESSIONID=qWexbn0dvI3DQwK0iJogbjyIZMyDPt6ZvFNRjf8C.prod-sis-elo-7985876df7-vjld8; production-sis-elo-stickiness=\"52b9de5ca4f0e8c4\";");

        www.downloadHandler = (DownloadHandler) new DownloadHandlerBuffer();
        
        www.SendWebRequest().completed += op =>
            {
                Debug.Log(www.downloadHandler.text);
            };
        
    }
}
