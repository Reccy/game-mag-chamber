using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameManager : MonoBehaviour {

    public bool debugMode = false;

    public float slowMotionTime = 1;
    public float slowMotionTimeTarget = 0.2f;

    //Creates psuedo-singleton pattern
	void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
	}

    void Start()
    {
        //Go to debug screen on startup
        if(debugMode)
        {
            SceneManager.LoadScene("debug");
        }
        else
        {
            SceneManager.LoadScene(1);
        }
    }

    //Enables slow motion
    public IEnumerator EnableSlowMotion(float duration)
    {
        float slowMotionTimeStart = slowMotionTime;
        float t = 0;
        while(slowMotionTime >= slowMotionTimeTarget)
        {
            t += Time.fixedDeltaTime / duration;
            slowMotionTime = Mathf.Lerp(slowMotionTimeStart, slowMotionTimeTarget, t);
            yield return new WaitForEndOfFrame();
        }
        if (slowMotionTime != slowMotionTimeTarget)
            slowMotionTime = slowMotionTimeTarget;
    }

    //Disables slow motion
    public IEnumerator DisableSlowMotion(float duration)
    {
        float slowMotionTimeStart = slowMotionTime;
        float t = 0;
        while (slowMotionTime <= 1)
        {
            t += Time.fixedDeltaTime / duration;
            slowMotionTime = Mathf.Lerp(slowMotionTimeStart, 1, t);
            yield return new WaitForEndOfFrame();
        }
        if (slowMotionTime != 1)
            slowMotionTime = 1;
    }
}
