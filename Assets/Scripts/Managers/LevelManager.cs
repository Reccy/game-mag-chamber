using UnityEngine;
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

    //TEST
    void Start()
    {
        StartLevel();
    }

    //Start the level
    public void StartLevel()
    {
        levelStartTime = Time.time;
        levelElapsedTime = 0;
        phaseIndex = 0;
        currentPhase = phases[phaseIndex];
        currentObstacles = 0;
        lastSpawnTime = 0;
        StartCoroutine(LevelManagerCoroutine());
    }

    //Select the next phase
    void NextPhase()
    {
        if(phaseIndex + 1 < phases.Length)
        {
            phaseIndex++;
        }
        else
        {
            Debug.LogError("At final phase. Cannot access next phase.");
        }
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
    public float nextPhaseTime; //Time in seconds until the next phase. Cumulative from last phase.
    public float phaseStartTime; //Time at which the phase started

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