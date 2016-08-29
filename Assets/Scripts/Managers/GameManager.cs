using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using MonsterLove.StateMachine;

public class GameManager : MonoBehaviour {

    //Game State
    public enum GameState { Initializing, Paused, Running, LoadingScene };
    public StateMachine<GameState> gameState;
    
    public bool debugMode = false;

    //Canvas Graphic Raycaster
    public GraphicRaycaster gfxRaycaster;

    //Slow-Motion Coroutine Variables
    public float slowMotionMultiplier = 1;
    public float slowMotionMultiplierTarget = 0.2f;
    Coroutine startSlowMotion, stopSlowMotion;

    //Scene Transition effect
    SceneTransition sceneTransition;
    bool sceneLoaded;

	void Awake()
    {
        //Get reference to scene transition effect
        sceneTransition = Object.FindObjectOfType<SceneTransition>();

        //Subscribe to Scene Manager callback
        SceneManager.activeSceneChanged += SceneManager_activeSceneChanged;

        //Init game state
        gameState = StateMachine<GameState>.Initialize(this);
        gameState.ChangeState(GameState.Initializing);

        //Is the scene finished loading
        sceneLoaded = false;

        //Creates psuedo-singleton pattern
        DontDestroyOnLoad(this.gameObject);
	}

    void Start()
    {
        //Go to debug screen on startup
        if(debugMode)
        {
            LoadScene("debug", 0, false);
        }
        else
        {
            LoadScene(1);
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

    public void TogglePause()
    {
        //Game pause
        if (gameState.State == GameManager.GameState.Running)
        {
            gameState.ChangeState(GameManager.GameState.Paused);
        }

        //Game resume
        else if (gameState.State == GameManager.GameState.Paused)
        {
            gameState.ChangeState(GameManager.GameState.Running);
        } 
    }

    //Scene Transition Methods
    public void LoadScene(string sceneName, float delay = 0, bool transition = true)
    {
        if(gameState.State != GameState.LoadingScene)
            StartCoroutine(LoadSceneCoroutine(sceneName, delay, transition));
    }

    public void LoadScene(int sceneIndex, float delay = 0, bool transition = true)
    {
        if (gameState.State != GameState.LoadingScene)
            StartCoroutine(LoadSceneCoroutine(sceneIndex, delay, transition));
    }

    IEnumerator LoadSceneCoroutine(string scene, float delay = 0, bool transition = true)
    {
        //Pre-Scene load
        gameState.ChangeState(GameState.LoadingScene);

        delay = delay + Time.time;

        if (gfxRaycaster)
            gfxRaycaster.enabled = false;

        while(delay > Time.time)
        {
            yield return new WaitForEndOfFrame();
        }

        if (transition)
        {
            sceneTransition.Transition();
        }
        
        while(sceneTransition.IsTransitioning())
        {
            yield return new WaitForEndOfFrame();
        }

        SceneManager.LoadScene(scene);
    }

    IEnumerator LoadSceneCoroutine(int scene, float delay = 0, bool transition = true)
    {
        //Pre-Scene load
        gameState.ChangeState(GameState.LoadingScene);

        delay = delay + Time.time;

        if (gfxRaycaster)
            gfxRaycaster.enabled = false;

        while (delay > Time.time)
        {
            yield return new WaitForEndOfFrame();
        }

        if (transition)
        {
            sceneTransition.Transition();
        }

        while (sceneTransition.IsTransitioning())
        {
            yield return new WaitForEndOfFrame();
        }

        SceneManager.LoadScene(scene);
    }

    IEnumerator PostLoadCoroutine()
    {
        if(SceneManager.GetSceneByName("preload").name != SceneManager.GetActiveScene().name)
        {
            if (GameObject.FindGameObjectWithTag("MainCanvas"))
            {
                gfxRaycaster = GameObject.FindGameObjectWithTag("MainCanvas").GetComponent<GraphicRaycaster>();
                gfxRaycaster.enabled = false;
            }

            if (!sceneTransition.IsTransitioning())
            {
                sceneTransition.Transition();
            }

            while (sceneTransition.IsTransitioning())
            {
                yield return new WaitForEndOfFrame();
            }

            gfxRaycaster.enabled = true;

            gameState.ChangeState(GameState.Running);
        }
    }

    //On scene change
    void SceneManager_activeSceneChanged(Scene arg0, Scene arg1)
    {
        StartCoroutine(PostLoadCoroutine());
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
