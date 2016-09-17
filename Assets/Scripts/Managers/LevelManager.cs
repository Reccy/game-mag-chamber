using UnityEngine;
using System.Collections;

public class LevelManager : MonoBehaviour
{
    //Phase management
    public Phase[] phases; //Array of level phases
    private int currentPhase; //Current phase of the level

    private float levelElapsedTime; //Time since beginning of level

    //Start the level again
    public void StartLevel()
    {
        levelElapsedTime = 0;
        currentPhase = 0;
    }

    
}

//A level phase
[System.Serializable]
public class Phase
{
    public Obstacle[] obstacles; //Array of obstacles in this phase
    public float patternSpawnRate; //Rate at which each new pattern spawns in seconds.
    public float nextPhaseTime; //Time in seconds until the next phase. Cumulative from last phase.
}


//An obstacle
[System.Serializable]
public class Obstacle
{
    public GameObject obstacleObject; //Obstacle game object
}