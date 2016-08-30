using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

[InitializeOnLoad]
public static class SimpleEditorUtils
{
    //Sets quality level to lowest in editor
	static SimpleEditorUtils() {
	    if(EditorApplication.isPlaying)
        {
            QualitySettings.antiAliasing = 0;
        }
	}

    //Go to the prelaunch scene and then play (Ctrl + 0)
    [MenuItem("Editor Tools/Play-Unplay, But From Prelaunch Scene %0")]
    public static void PlayFromPrelaunchScene()
    {
        if (EditorApplication.isPlaying == true)
        {
            EditorApplication.isPlaying = false;
            return;
        }

        PlayerPrefs.SetString("DEBUG_PreviousScene", EditorSceneManager.GetActiveScene().path);
        PlayerPrefs.Save();

        EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
        EditorSceneManager.OpenScene("Assets/Scenes/preload.unity");
        EditorApplication.isPlaying = true;
    }

    //Reload last edited scene
    [MenuItem("Editor Tools/Edit Previous Played Scene %1")]
    public static void GoToLastScene()
    {
        if (EditorApplication.isPlaying == true)
        {
            EditorApplication.isPlaying = false;
            return;
        }
        if(PlayerPrefs.HasKey("DEBUG_PreviousScene"))
        {
            EditorSceneManager.OpenScene(PlayerPrefs.GetString("DEBUG_PreviousScene"));
        }
        else
        {
            Debug.Log("Error: No previous scene found!");
        }
    }

    //Refresh Asset Database
    [MenuItem("Editor Tools/Refresh Asset Database")]
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

    //Delete all player prefs
    [MenuItem("Editor Tools/Delete Player Prefs")]
    public static void DeletePlayePrefs()
    {
        if (EditorApplication.isPlaying == true)
        {
            EditorApplication.isPlaying = false;
            return;
        }
        if(EditorUtility.DisplayDialog("Delete PlayerPrefs?", "Are you sure you want to delete all PlayerPrefs?", "Delete PlayerPrefs", "Cancel"))
        {
            PlayerPrefs.DeleteAll();
        }
    }

    //Opens the project in the explorer
    [MenuItem("Editor Tools/Open Project in Explorer")]
    private static void OpenInExplorer()
    {
        string itemPath = Application.dataPath;
        itemPath = itemPath.Replace(@"/", @"\");   // explorer doesn't like front slashes
        System.Diagnostics.Process.Start("explorer.exe", "/select," + itemPath);
    }
}