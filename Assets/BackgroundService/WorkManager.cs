using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UIElements;

public class WorkManager : MonoBehaviour
{
    //start a background service for android
    public void StartService()
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject jo = jc.GetStatic<AndroidJavaObject>("currentActivity");
            jo.Call("StartService");
        }
    }
    
    //stop a background service for android
    public void StopService()
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject jo = jc.GetStatic<AndroidJavaObject>("currentActivity");
            jo.Call("StopService");
        }
    }
    
    //run a function every 1 minute for android
    public void StartTimer()
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject jo = jc.GetStatic<AndroidJavaObject>("currentActivity");
            jo.Call("StartTimer");
        }
    }
    
    //implement StartTimer
    public void StartTimerCallback()
    {
        Debug.Log("StartTimerCallback");
    }
}