using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameManager : MonoBehaviour {

    public bool debugMode = false;

    //Slow-Motion Coroutine Variables
    public float slowMotionMultiplier = 1;
    public float slowMotionMultiplierTarget = 0.2f;
    Coroutine startSlowMotion;
    Coroutine stopSlowMotion;

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

    //Slow Motion Methods
    public void EnableSlowMotion(float duration)
    {
        if (startSlowMotion == null)
            startSlowMotion = StartCoroutine(EnableSlowMotionCoroutine(duration));

        if (stopSlowMotion != null)
        {
            StopCoroutine(stopSlowMotion);
            stopSlowMotion = null;
        }
    }

    public void DisableSlowMotion(float duration)
    {
        if (stopSlowMotion == null)
            stopSlowMotion = StartCoroutine(DisableSlowMotionCoroutine(duration));

        if (startSlowMotion != null)
        {
            StopCoroutine(startSlowMotion);
            startSlowMotion = null;
        }
    }

    //Enables slow motion
    IEnumerator EnableSlowMotionCoroutine(float duration)
    {
        float slowMotionTimeStart = slowMotionMultiplier;
        float t = 0;
        while(slowMotionMultiplier >= slowMotionMultiplierTarget)
        {
            t += Time.fixedDeltaTime / duration;
            slowMotionMultiplier = Mathf.Lerp(slowMotionTimeStart, slowMotionMultiplierTarget, t);
            yield return new WaitForFixedUpdate();
        }
        if (slowMotionMultiplier != slowMotionMultiplierTarget)
            slowMotionMultiplier = slowMotionMultiplierTarget;
    }

    //Disables slow motion
    IEnumerator DisableSlowMotionCoroutine(float duration)
    {
        float slowMotionTimeStart = slowMotionMultiplier;
        float t = 0;
        while (slowMotionMultiplier <= 1)
        {
            t += Time.fixedDeltaTime / duration;
            slowMotionMultiplier = Mathf.Lerp(slowMotionTimeStart, 1, t);
            yield return new WaitForFixedUpdate();
        }
        if (slowMotionMultiplier != 1)
            slowMotionMultiplier = 1;
    }
}
