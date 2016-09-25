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
    public Text timeText; //UI text to display time

    //DEBUG
    void Start()
    {
        StartLevel();
    }

    //Start the level
    public void StartLevel()
    {
        levelStartTime = Time.time;
        levelElapsedTime = 0;
        phaseIndex = -1;
        currentPhase = phases[0];
        currentObstacles = 0;
        lastSpawnTime = 0;
        accumulatedPhaseTime = currentPhase.phaseDuration;
        NextPhase();
        StartCoroutine(LevelManagerCoroutine());
    }

    //Go to next phase
    void NextPhase()
    {
        //If there is a next phase, go to next phase
        if(HasNextPhase())
        {
            phaseIndex++;
            currentPhase = phases[phaseIndex];
            accumulatedPhaseTime += currentPhase.phaseDuration;

            //Phase dependant code execution
            switch (phaseIndex)
            {
                case 0:
                    Debug.Log("FIRST!!!!11!!");
                    break;
                case 1:
                    Debug.Log("second");
                    break;
                case 2:
                    Debug.Log("second (index) ;)");
                    break;
                case 3:
                    Debug.Log("THIRD INDEX! :D");
                    break;
            }
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

                //Give reference to level manager
                obstacleObject.GetComponent<BulletPattern>().SetLevelManager(this);

                //Update last spawn time
                lastSpawnTime = Time.time;

                //Increment phase obstacles
                currentObstacles++;
            }

            //Update level elapsed time
            levelElapsedTime = Time.time - levelStartTime;

            //Go to next phase when ready
            if(currentPhase.phaseDuration != 0 && levelElapsedTime >= accumulatedPhaseTime && HasNextPhase())
            {
                NextPhase();
            }

            if(HasNextPhase())
                Debug.Log("Current Phase: " + phaseIndex + " || Current Time: " + levelElapsedTime + " || Change Time: " + accumulatedPhaseTime);
            
            //Update UI Time
            int minutes, seconds, timeInSeconds;
            timeInSeconds = (int)Mathf.Floor(levelElapsedTime);
            minutes = (int)Mathf.Floor(timeInSeconds / 60);
            seconds = timeInSeconds - (minutes * 60);
            if(minutes < 10)
            {
                if(seconds < 10)
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