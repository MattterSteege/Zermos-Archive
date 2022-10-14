using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class screenManager : MonoBehaviour
{
    void Start()
    {
#if UNITY_ANDROID
        Screen.fullScreen = false;
#endif
    }
}
