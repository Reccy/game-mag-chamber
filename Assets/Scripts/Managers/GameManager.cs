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
