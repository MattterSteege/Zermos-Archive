#if UNITY_EDITOR
using UnityEditor;

public class SaveLocalPrefs : AssetModificationProcessor
{
    static string[] OnWillSaveAssets(string[] paths)
    {
        LocalPrefs.Save();
        return paths;
    }
}
#endif
