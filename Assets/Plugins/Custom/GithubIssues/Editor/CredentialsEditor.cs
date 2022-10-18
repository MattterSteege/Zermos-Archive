using System;
using System.Collections.Generic;
using Octokit;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace GithubIssues
{
    public class CredentialsEditor : EditorWindow
    {
        private TextField _repoURL;
        private TextField _token;

        private Editor _settingEditor;
        private SettingsScriptableObject _settingsScriptableObject;
        
        private static readonly string[] _dontIncludeMe = new string[]{"m_Script"};

        [MenuItem("Github For Unity/Credentials", false, -1)]
        public static void ShowExample()
        {
            CredentialsEditor wnd = GetWindow<CredentialsEditor>();
            wnd.titleContent = new GUIContent("Github Credentials");

            wnd.minSize = new Vector2(540, 60);
            wnd.maxSize = new Vector2(540, 60);
        }

        private void OnGUI()
        {
            if (_settingEditor != null)
            {
                _settingEditor.OnInspectorGUI();
            }
        }

        private void OnEnable()
        {
            _settingsScriptableObject =
                AssetDatabase.LoadAssetAtPath<SettingsScriptableObject>(
                    "Assets/Plugins/Custom/GithubIssues/Editor/Settings.asset");

            if (_settingsScriptableObject == null)
            {
                _settingsScriptableObject = ScriptableObject.CreateInstance<SettingsScriptableObject>();
                AssetDatabase.CreateAsset(_settingsScriptableObject, "Assets/Plugins/Custom/GithubIssues/Editor/Settings.asset");
            }

            _settingEditor = Editor.CreateEditor(_settingsScriptableObject);
        }

        private void OnDisable()
        {
            if (_settingEditor != null)
            {
                _settingsScriptableObject.username = _settingsScriptableObject.repoURL.Replace("https://", "").Trim().Split('/')[1];
                _settingsScriptableObject.repoName = _settingsScriptableObject.repoURL.Replace("https://", "").Trim().Split('/')[2];

                _settingsScriptableObject.client = new GitHubClient(new ProductHeaderValue(_settingsScriptableObject.repoName ?? "Unity-Game"));

                var tokenAuth = new Credentials(_settingsScriptableObject.token);
                _settingsScriptableObject.client.Credentials = tokenAuth;
            }
        }
    }
    
    
}