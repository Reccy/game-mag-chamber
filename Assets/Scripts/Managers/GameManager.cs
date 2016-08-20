using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameManager : MonoBehaviour {

    public bool debugMode = false;

	void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
	}

    void Start()
    {
        if (Application.isEditor)
        {
            QualitySettings.antiAliasing = 0;
            QualitySettings.vSyncCount = 0;
        }

        if(debugMode)
        {
            SceneManager.LoadScene("debug");
        }
        else
        {
            SceneManager.LoadScene(1);
        }
    }
}
