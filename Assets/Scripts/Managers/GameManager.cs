using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using MonsterLove.StateMachine;

public class GameManager : MonoBehaviour {

    //Game State
    public enum GameState { Initializing, Menu, Paused, Running, LoadingScene };
    public StateMachine<GameState> gameState;
    
    public bool debugMode = false;

    //Slow-Motion Coroutine Variables
    public float slowMotionMultiplier = 1;
    public float slowMotionMultiplierTarget = 0.2f;
    Coroutine startSlowMotion, stopSlowMotion, loadScene;

    //Scene Transition effect
    SceneTransition sceneTransition;

	void Awake()
    {
        //Get reference to scene transition effect
        sceneTransition = Object.FindObjectOfType<SceneTransition>();

        //Subscribe to Scene Manager callback
        SceneManager.activeSceneChanged += SceneManager_activeSceneChanged;

        //Init game state
        gameState = StateMachine<GameState>.Initialize(this);
        gameState.ChangeState(GameState.Initializing);

        //Creates psuedo-singleton pattern
        DontDestroyOnLoad(this.gameObject);
	}

    void Start()
    {
        gameState.ChangeState(GameState.Running);
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

    void Update()
    {
        if(debugMode)
        {
            if(Input.GetKey(KeyCode.LeftControl))
            {
                if(Input.GetKeyDown(KeyCode.L))
                {
                    LoadScene("debug");
                }
            }
        }
    }

    //Paused methods
    void Paused_Enter()
    {
        Time.timeScale = 0;
    }

    void Paused_Exit()
    {
        Time.timeScale = 1;
    }

    //Scene Transition Methods
    public void LoadScene(string sceneName, float delay = 0)
    {
        if (loadScene == null)
            StartCoroutine(LoadSceneCoroutine(sceneName, delay));
        else
            Debug.Log("ERROR! Scene is already being loaded!");
    }

    IEnumerator LoadSceneCoroutine(string sceneName, float delay = 0)
    {
        delay = delay + Time.time;

        while(delay > Time.time)
        {
            yield return new WaitForEndOfFrame();
        }

        sceneTransition.Transition();
        
        while(sceneTransition.IsTransitioning())
        {
            yield return new WaitForEndOfFrame();
        }

        SceneManager.LoadScene(sceneName);
    }

    void SceneManager_activeSceneChanged(Scene arg0, Scene arg1)
    {
        if (!sceneTransition.IsTransitioning())
        {
            sceneTransition.Transition();
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
