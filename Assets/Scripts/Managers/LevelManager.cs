using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class LevelManager : MonoBehaviour
{
    //Phase management
    public Phase[] phases; //Array of level phases
    public Vector2[] spawnLocations; //Locations that the obstacles can spawn at
    private int currentPhase; //Current phase of the level (0 = no phase)

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
        currentPhase = 0;
        lastSpawnTime = 0;
        StartCoroutine(LevelManagerCoroutine());
    }

    //Manages the level's progression
    IEnumerator LevelManagerCoroutine()
    {
        while(true)
        {
            //Loop through each phase
            for (int i = 0; i <= currentPhase; i++)
            {
                //Get reference to spawn object and obstacle
                Vector2 spawnPoint = GetSpawnPoint();
                Obstacle obstacle = phases[i].GetRandomObstacle();

                //Check if it is time to spawn another obstacle
                if (phases[i].currentObstacles < phases[i].maxObstacles && (Time.time - lastSpawnTime) >= phases[i].patternSpawnRate)
                {
                    //Instantiate an obstacle
                    Instantiate(obstacle.obstacleObject, spawnPoint, Quaternion.identity);

                    //Update last spawn time
                    lastSpawnTime = Time.time;

                    //Increment phase obstacles
                    phases[i].currentObstacles++;
                }
            }

            //Update level elapsed time
            levelElapsedTime = Time.time - levelStartTime;

            yield return new WaitForEndOfFrame();
        }
    }

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
    public int currentObstacles; //Current amount of obstacles spawned in this phase
    public int maxObstacles; //Maximum amount of obstacles allowed to be spawned at once
    public float patternSpawnRate; //Rate at which each new pattern spawns in seconds.
    public float nextPhaseTime; //Time in seconds until the next phase. Cumulative from last phase.
    private float phaseStartTime; //Time at which the phase started

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