using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class LevelManager : MonoBehaviour
{
    //Phase management
    public Phase[] phases; //Array of level phases
    public Vector2[] spawnLocations; //Locations that the obstacles can spawn at
    private Phase currentPhase; //Current phase of the level
    private int phaseIndex; //Current phase as index
    private int currentObstacles; //Current quantity of obstacles spawned

    //Time management
    private float levelStartTime; //Time since beginning of level
    private float levelElapsedTime; //Time since beginning of level
    private float lastSpawnTime; //Time when last obstacle was spawned
    private float accumulatedPhaseTime; //Accumulated time from phaseDuration
    private float timeInSeconds; //Level elapsed time accounted for by deltaTime
    public Text timeText; //UI text to display time
    public Text timeMillisecondsText; //UI text to display milliseconds
    public bool timerRunning = true; //If the timer should continue to run

    //UI management
    public GameObject gameUI; //Game UI
    public Canvas menuUI; //Menu UI
    MainMenu mainMenu;

    //Reference to player
    GameObject player;

    //Reference to Game Manager
    GameManager gameManager;
    SoundManager sound;

    //Get instances to objects and hide game UI
    void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        gameManager = Object.FindObjectOfType<GameManager>();
        sound = Object.FindObjectOfType<SoundManager>();
        mainMenu = Object.FindObjectOfType<MainMenu>();
        if(gameManager.newScore)
        {
            menuUI.transform.Find("BestTimeBG").Find("BestTimeText").gameObject.GetComponent<Text>().text = "New Best Time!";
        }
        gameUI.SetActive(false);
    }

    //Start the level
    public void StartLevel()
    {
        gameManager.StartLevelState();
        levelStartTime = Time.time;
        levelElapsedTime = 0;
        phaseIndex = 0;
        currentPhase = phases[0];
        currentObstacles = 0;
        lastSpawnTime = 0;
        accumulatedPhaseTime = currentPhase.phaseDuration;
        StartCoroutine(LevelManagerCoroutine());
        gameUI.SetActive(true);
        
        if(GameObject.Find("ClickText"))
            GameObject.Find("ClickText").SetActive(false);

        if (GameObject.Find("BestTimeBG"))
            GameObject.Find("BestTimeBG").SetActive(false);

        if (GameObject.Find("BeginButton"))
            GameObject.Find("BeginButton").SetActive(false);

        menuUI.enabled = false;
        sound.Play("sfx_Mayhem", SoundManager.SoundChannel.Music, 0.8f, true, 0, 128, true);
    }

    //Go to next phase
    void NextPhase()
    {
        //If there is a next phase, go to next phase
        if(HasNextPhase())
        {
            Debug.Log("NEXT PHASE! " + timeInSeconds);
            phaseIndex++;
            currentPhase = phases[phaseIndex];
            accumulatedPhaseTime += currentPhase.phaseDuration;
        }
    }

    //Is there another phase in the next position of the phases index
    bool HasNextPhase()
    {
        if(phaseIndex + 1 < phases.Length)
        {
            return true;
        }
        return false;
    }

    //Manages the level's progression
    IEnumerator LevelManagerCoroutine()
    {
        while(true)
        {
            //Get reference to spawn object and obstacle
            Vector2 spawnPoint = GetSpawnPoint();
            Obstacle obstacle = currentPhase.GetRandomObstacle();

            //Check if it is time to spawn another obstacle
            if (currentObstacles < currentPhase.maxObstacles && (Time.time - lastSpawnTime) >= currentPhase.patternSpawnRate)
            {
                //Instantiate an obstacle
                GameObject obstacleObject = Instantiate(obstacle.obstacleObject, spawnPoint, Quaternion.identity) as GameObject;

                //Give reference to game manager
                obstacleObject.GetComponent<BulletPattern>().GameManager = gameManager;

                //Give reference to level manager
                obstacleObject.GetComponent<BulletPattern>().LevelManager = this;

                //Give reference to player
                obstacleObject.GetComponent<BulletPattern>().Player = player;

                //Give reference to sound manager
                obstacleObject.GetComponent<BulletPattern>().SoundManager = sound;

                //Update last spawn time
                lastSpawnTime = Time.time;

                //Increment phase obstacles
                currentObstacles++;
            }

            //Update level elapsed time
            levelElapsedTime = timeInSeconds;

            //Error checking for negative values
            if (levelElapsedTime < 0)
                levelElapsedTime = 0;

            //Go to next phase when ready
            if(currentPhase.phaseDuration != 0 && levelElapsedTime >= accumulatedPhaseTime && HasNextPhase())
            {
                NextPhase();
            }
            
            //Update UI Time
            if(timerRunning)
            {
                int minutes, seconds;
                float milliseconds;
                string millisecondsString;
                timeInSeconds += (Time.deltaTime * gameManager.slowMotionMultiplier);
                minutes = (int)Mathf.Floor(timeInSeconds / 60);
                seconds = (int)Mathf.Floor(timeInSeconds) - (minutes * 60);
                milliseconds = timeInSeconds - seconds;

                millisecondsString = milliseconds.ToString() + "00000";
                millisecondsString = millisecondsString.Substring(1, 4);
                timeMillisecondsText.text = millisecondsString;

                if (minutes < 10)
                {
                    if (seconds < 10)
                    {
                        timeText.text = "0" + minutes + ":0" + seconds;
                    }
                    else
                    {
                        timeText.text = "0" + minutes + ":" + seconds;
                    }
                }
                else
                {
                    if (seconds < 10)
                    {
                        timeText.text = minutes + ":0" + seconds;
                    }
                    else
                    {
                        timeText.text = minutes + ":" + seconds;
                    }
                }
            }

            yield return new WaitForFixedUpdate();
        }
    }

    //Destroy callback for Obstacles
    public void DestroyCallback(BulletPattern td)
    {
        currentObstacles--;
    }

    //Returns a random spawn point
    private Vector2 GetSpawnPoint()
    {
        return spawnLocations[Random.Range(0, spawnLocations.Length)];
    }

    public void ShowMenu()
    {
        menuUI.enabled = true;
    }

    public void HideMenu()
    {
        menuUI.enabled = false;
    }

    public float GetHighScore()
    {
        return timeInSeconds;
    }

    //Play UI sounds
    public void PlaySound_Click()
    {
        sound.PlayOneShot("sfx_Click", SoundManager.SoundChannel.UI);
    }

    public void PlaySound_DropdownHover()
    {
        if(sound && mainMenu.isInitialized)
            sound.PlayOneShot("sfx_Click", SoundManager.SoundChannel.UI, 0.2f);
    }

    //Makes the high score highlight on player death
    public void HighlightScore()
    {
        StartCoroutine(HighlightScoreCoroutine());
    }

    private IEnumerator HighlightScoreCoroutine()
    {
        Color targetColor = new Color(timeText.color.r, timeText.color.g, timeText.color.b, 1);
        float alpha;
        while(true)
        {
            if (timeText.color != targetColor)
            {
                alpha = timeText.color.a + 0.05f;
                timeText.color = new Color(timeText.color.r, timeText.color.g, timeText.color.b, Mathf.Clamp(alpha, 0, 1));
                timeMillisecondsText.color = new Color(timeMillisecondsText.color.r, timeMillisecondsText.color.g, timeMillisecondsText.color.b, Mathf.Clamp(alpha, 0, 1));
            }
            else
            {
                yield break;
            }
            yield return new WaitForFixedUpdate();
        }
    }
}

//A level phase
[System.Serializable]
public class Phase
{
    private LevelManager levelManager;

    public Obstacle[] obstacles; //Array of obstacles in this phase
    public int maxObstacles; //Maximum amount of obstacles allowed to be spawned at once
    public float patternSpawnRate; //Rate at which each new pattern spawns in seconds.
    public float phaseDuration; //Time in seconds until the next phase. Cumulative from last phase.
    [HideInInspector]
    public float phaseStartTime; //Time at which the phase started

    //Returns a random obstacle within the phase
    public Obstacle GetRandomObstacle()
    {
        return obstacles[Random.Range(0, obstacles.Length)];
    }
}

//A bullet hell pattern
[System.Serializable]
public class Obstacle
{
    public GameObject obstacleObject; //Obstacle game object
}