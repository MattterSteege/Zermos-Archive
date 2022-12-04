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

        [MenuItem("Tools/Github Issues/Credentials")]
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
                    "Assets/Plugins/GithubIssues/Editor/Settings.asset");

            if (_settingsScriptableObject == null)
            {
                _settingsScriptableObject = ScriptableObject.CreateInstance<SettingsScriptableObject>();
                AssetDatabase.CreateAsset(_settingsScriptableObject, "Assets/Plugins/GithubIssues/Editor/Settings.asset");
            }

            _settingEditor = Editor.CreateEditor(_settingsScriptableObject);
        }
    }
}