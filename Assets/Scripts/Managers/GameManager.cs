using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using MonsterLove.StateMachine;

public class GameManager : MonoBehaviour {

    //Game State
    public enum GameState { Initializing, Paused, Running, MainMenu, LoadingScene };
    public StateMachine<GameState> gameState;
    
    public bool debugMode = false;
    public GameObject debugMenuPrefab;
    private GameObject debugMenuInstance;

    //Canvas Graphic Raycaster
    [HideInInspector]
    public GraphicRaycaster gfxRaycaster;

    //Slow-Motion Coroutine Variables
    public float slowMotionMultiplier = 1;
    public float slowMotionMultiplierTarget = 0.2f;
    Coroutine startSlowMotion, stopSlowMotion;

    //Level transitions
    public GameObject introTransition, outroTransition;
    GameObject introTransitionInstance, outroTransitionInstance;

    LevelManager levelManager;
    public bool newScore = false;

	void Awake()
    {
        //Subscribe to Scene Manager callback
        SceneManager.activeSceneChanged += SceneManager_activeSceneChanged;

        //Init game state
        gameState = StateMachine<GameState>.Initialize(this);
        gameState.ChangeState(GameState.Initializing);

        //Transition instances should be null
        introTransitionInstance = null;
        outroTransitionInstance = null;

        //Creates psuedo-singleton pattern
        DontDestroyOnLoad(this.gameObject);
	}

    void Start()
    {
        LoadScene(1, 0, false);
    }

    void Update()
    {
        //Debug Mode code - Only runs when Debug Mode is enabled from the _app object
        if(debugMode)
        {
            if(Input.GetKey(KeyCode.LeftControl))
            {
                if(Input.GetKeyDown(KeyCode.D))
                {
                    ToggleDebugMenu();
                }
            }
        }
    }

    //Debug Menu
    void ToggleDebugMenu()
    {
        if(Camera.main && !debugMenuInstance)
        {
            debugMenuInstance = (GameObject)Instantiate(debugMenuPrefab, Vector2.zero, Quaternion.identity, GameObject.FindGameObjectWithTag("MainCanvas").transform.Find("CanvasPanel").transform);
        }
        else if (Camera.main && debugMenuInstance)
        {
            if(debugMenuInstance.activeInHierarchy)
            {
                debugMenuInstance.SetActive(false);
            }
            else
            {
                debugMenuInstance.SetActive(true);
            }
        }
    }

    //Paused methods
    void Paused_Enter()
    {
        Time.timeScale = 0;
        levelManager.ShowMenu();
    }

    void Paused_Exit()
    {
        Time.timeScale = 1;
        levelManager.HideMenu();
    }

    public void StartLevelState()
    {
        gameState.ChangeState(GameManager.GameState.Running);
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

        if (SceneManager.GetActiveScene().buildIndex != 0)
        {
            float secondDelay = Time.time + 0.3f;

            FadeOutAnimation();

            while (secondDelay > Time.time)
            {
                yield return new WaitForEndOfFrame();
            }
        }

        introTransitionInstance = null;
        outroTransitionInstance = null;
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

        if(SceneManager.GetActiveScene().buildIndex != 0)
        {
            float secondDelay = Time.time + 0.3f;

            FadeOutAnimation();

            while (secondDelay > Time.time)
            {
                yield return new WaitForEndOfFrame();
            }
        }
        

        introTransitionInstance = null;
        outroTransitionInstance = null;
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

            gfxRaycaster.enabled = true;

            levelManager = Object.FindObjectOfType<LevelManager>();

            if (SceneManager.GetActiveScene().buildIndex != 1)
                FadeInAnimation();

            gameState.ChangeState(GameState.MainMenu);

            yield break;
        }
    }

    public void FadeInAnimation()
    {
        Transform mainCanvas;
        if(SceneManager.GetActiveScene().name == "Level1")
        {
            mainCanvas = GameObject.Find("ScreenCanvas").transform;
        }
        else
        {
            mainCanvas = GameObject.FindGameObjectWithTag("MainCanvas").transform.Find("CanvasPanel");
        }
        
        if(introTransitionInstance == null)
            introTransitionInstance = Instantiate(introTransition, Vector3.zero, Quaternion.identity, mainCanvas) as GameObject;
    }

    public void FadeOutAnimation()
    {
        Transform mainCanvas;
        if (SceneManager.GetActiveScene().name == "Level1")
        {
            mainCanvas = GameObject.Find("TransitionCanvas").transform;
        }
        else
        {
            mainCanvas = GameObject.FindGameObjectWithTag("MainCanvas").transform.Find("CanvasPanel");
        }

        if(outroTransitionInstance == null)
            outroTransitionInstance = Instantiate(outroTransition, Vector3.zero, Quaternion.identity, mainCanvas) as GameObject;
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

    public bool SlowMotionEnabled()
    {
        if(startSlowMotion != null || slowMotionMultiplier == slowMotionMultiplierTarget)
            return true;
        return false;
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
