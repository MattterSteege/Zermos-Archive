using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class GithubIssues : EditorWindow
{
    private static string _url = "https://github.com/MJTSgamer/Zermos/issues";
    
    [MenuItem("Tools/Github Issues", false, 10000)]
    public static void ShowWindow()
    {
        Application.OpenURL(_url);
    }
}
