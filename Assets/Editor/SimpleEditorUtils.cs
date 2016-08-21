﻿using UnityEditor;
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

    //Delete all player prefs
    [MenuItem("Editor Tools/Delete Player Prefs %2")]
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