using System.Linq;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using UnityEngine.UI;


public class FindComponentsInScene : EditorWindow
{
    [MenuItem("Window/Find Components In Scene")]
    public static void ShowExample()
    {
        FindComponentsInScene wnd = GetWindow<FindComponentsInScene>();
        wnd.titleContent = new GUIContent("FindComponentsInScene");
    }

    public void CreateGUI()
    {
        //find all components in the scene
        var components = FindObjectsOfType<Text>().ToList();
        
       Debug.Log($"{components.Count}");
    }
}