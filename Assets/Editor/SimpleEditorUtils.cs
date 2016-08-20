using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

[InitializeOnLoad]
public static class SimpleEditorUtils
{

    //Go to the prelaunch scene and then play (Ctrl + 0)
    [MenuItem("Editor Tools/Play-Unplay, But From Prelaunch Scene %0")]
    public static void PlayFromPrelaunchScene()
    {
        if (EditorApplication.isPlaying == true)
        {
            EditorApplication.isPlaying = false;
            return;
        }
        EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
        EditorSceneManager.OpenScene("Assets/Scenes/preload.unity");
        EditorApplication.isPlaying = true;
    }

    //Refresh Asset Database (Ctrl + 1)
    [MenuItem("Editor Tools/Refresh Asset Database %1")]
    public static void RefreshAssetDatabase()
    {
        if (EditorApplication.isPlaying == true)
        {
            EditorApplication.isPlaying = false;
            return;
        }
        AssetDatabase.Refresh();
        Debug.Log("Refreshed Asset Database!");
    }
}