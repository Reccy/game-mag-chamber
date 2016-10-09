using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using MonsterLove.StateMachine;
using UnityExtensions.Physics2DExtensions;

public class Player : MonoBehaviour
{
    //Manager variables
    InputManager inputManager;
    GameManager gameManager;
    MainCameraManager cameraManager;
    SoundManager sound;
    LevelManager levelManager;

    //State machine variables
    public enum State { Stationary, MovingNormal, MovingFast, MovingRedirected };
    StateMachine<State> playerState;

    //Other variables
    public Color lineRendererNormalStart, lineRendererNormalEnd, lineRendererBlockedStart, lineRendererBlockedEnd;
    public Sprite greenShield, redShield, greenGlow, redGlow;
    public GameObject playerImpactParticles, playerDeathParticles;
    SpriteRenderer shield, glow;
    LineRenderer lineRenderer;
    CircleCollider2D col;
    GameObject generator;
    GameObject eye;
    List<GameObject> collidingObjects; //List of objects the player is currently colliding with

    //Platform variables
    GameObject platform;
    Vector2 platformOffset;
    Quaternion platformRotation;

    Quaternion movingRotation; //Rotation for player's movement vector

    public float generatorRotationSpeed = 50;
    public float playerSpeed = 35;
    public float fastSpeed = 45;
    public float lineRendererOffset = 0.20f;
    public float canJumpDistance = 5;

    float generatorRotationPercent = 1; //Percentage to rotate the generator
    float collisionRadius; //Offset for manual collision detection

    void Awake()
    {
        //Get references to objects in scene
        inputManager = Object.FindObjectOfType<InputManager>();
        gameManager = Object.FindObjectOfType<GameManager>();
        cameraManager = Object.FindObjectOfType<MainCameraManager>();
        sound = Object.FindObjectOfType<SoundManager>();
        levelManager = Object.FindObjectOfType<LevelManager>();
        shield = transform.Find("Shield").GetComponent<SpriteRenderer>();
        glow = transform.Find("Glow").GetComponent<SpriteRenderer>();
        lineRenderer = GetComponent<LineRenderer>();
        col = GetComponent<CircleCollider2D>();
        generator = transform.Find("Generator").gameObject;
        eye = transform.Find("Eye").gameObject;
        collidingObjects = new List<GameObject>();

        //Set collision radius
        collisionRadius = col.radius + 0.01f;

        //Set the sorting layer for the lineRenderer (Prevents bug that causes line renderer to conflict with trail renderer)
        lineRenderer.sortingLayerName = "Game";

        //Init state machine
        playerState = StateMachine<State>.Initialize(this);
        playerState.ChangeState(State.Stationary);
    }

    //Universal Fixed Update
    void FixedUpdate()
    {
        //Rotate generator
        generator.transform.Rotate(Vector3.forward, generatorRotationSpeed * generatorRotationPercent * Time.deltaTime); 

        //Move eye to follow mouse
        eye.transform.position = transform.position;
        eye.transform.Translate(inputManager.GetMouseQuaternionFrom(this.gameObject) * (Vector2.up * Mathf.Clamp(Vector2.Distance((Vector2)transform.position, inputManager.GetMousePosition()), 0, 0.08f)));

        //Kill the player if they go out of bounds
        if(transform.position.x < -20 || transform.position.x > 20 || transform.position.y < -20 || transform.position.y > 20)
        {
            Die();
        }
    }

    //Stationary State
    void Stationary_Enter()
    {
        LineRendererEnabled(true);

        //Prevents sound from playing on level load
        if(playerState.LastState != State.Stationary)
        {
            sound.PlayOneShot("sfx_Land", SoundManager.SoundChannel.SFX, 0.8f, false, 0.3f, 128);
        }
    }

    void Stationary_Update()
    {
        //Handle Input
        if(inputManager.GetJumpButtonDown() && PlayerCanJump()) //LMB Down -> Moving Normal
        {
            playerState.ChangeState(State.MovingNormal);
        }
        UpdateLineRenderer();
    }

    void Stationary_FixedUpdate()
    {
        //If the player is on a platform, stay on the platform
        if(platform)
        {
            Quaternion rotationDifference = Quaternion.Inverse(platformRotation) * platform.transform.rotation;
            
            if (rotationDifference != Quaternion.identity)
            {
                transform.position = ((Vector2)platform.transform.position + (Vector2)(rotationDifference * platformOffset));
            }
            else
            {
                transform.position = ((Vector2)platform.transform.position + platformOffset);
            }
        }

        //Reset the generator rotation percentage over time
        if(generatorRotationPercent > 1)
        {
            generatorRotationPercent -= 20 * Time.deltaTime;
        }
        else if(generatorRotationPercent < 1)
        {
            generatorRotationPercent = 1;
        }
    }

    void Stationary_Finally()
    {
        platform = null; //Player is no longer on a platform
        generatorRotationPercent = 30; //Generator is at full power
    }

    //MovingNormal State
    void MovingNormal_Enter()
    {
        movingRotation = inputManager.GetMouseQuaternionFrom(this.gameObject);
        sound.PlayOneShot("sfx_Jump", SoundManager.SoundChannel.SFX, 0.9f, false, 0.3f, 128);
        gameManager.EnableSlowMotion(0.4f);
    }

    void MovingNormal_Update()
    {
        UpdateLineRenderer();

        //Handle Input
        if (inputManager.GetJumpButtonDown() && PlayerCanJump()) //LMB Up -> Jump Redirect
        {
            playerState.ChangeState(State.MovingRedirected);
        }
    }

    void MovingNormal_FixedUpdate()
    {
        //Move forward
        transform.Translate(movingRotation * (Vector2.up * playerSpeed * gameManager.slowMotionMultiplier * Time.deltaTime));
    }

    //MovingRedirected State
    void MovingRedirected_Enter()
    {
        LineRendererEnabled(false);
        gameManager.DisableSlowMotion(0);
        movingRotation = inputManager.GetMouseQuaternionFrom(this.gameObject);
    }

    void MovingRedirected_FixedUpdate()
    {
        //Move forward
        transform.Translate(movingRotation * (Vector2.up * fastSpeed * gameManager.slowMotionMultiplier * Time.deltaTime));
    }

    //Collision Detection Code
    //Trigger Enter Switch
    void OnTriggerEnter2D(Collider2D colObj)
    {
        string colLayer = LayerMask.LayerToName(colObj.gameObject.layer);

        switch (colLayer)
        {
            case "Platform":
                HandleCollisionEnter_Platform(colObj);
                break;
            case "Enemy":
                HandleCollisionEnter_Enemy(colObj);
                break;
        }
    }

    //Trigger Exit Switch
    void OnTriggerExit2D(Collider2D colObj)
    {
        string colLayer = LayerMask.LayerToName(colObj.gameObject.layer);

        switch (colLayer)
        {
            case "Platform":
                HandleCollisionExit_Platform(colObj);
                break;
        }
    }

    //Collision with platform enter
    void HandleCollisionEnter_Platform(Collider2D platformCollision)
    {
        if (!collidingObjects.Contains(platformCollision.transform.gameObject))
        {
            collidingObjects.Add(platformCollision.transform.gameObject); //Add object to list of colliding objects
        }

        RaycastHit2D colCast = Physics2DExtensions.ArcCast(transform.position, 0, 360, 360, collisionRadius, LayerMask.GetMask("Platform"));

        if(colCast)
        {
            if (colCast.transform.gameObject != platform) //If the player collides with a different platform
            {
                cameraManager.ScreenShake(0.2f); //Screen shake

                //Set player position
                platform = colCast.transform.gameObject; //Set the connected platform
                transform.position = colCast.point + (colCast.normal * col.radius); //Set the player's position to the platform surface
                platformOffset = transform.position - platform.transform.position; //Get the offset relative to the platform's center
                platformRotation = platform.transform.rotation; //Get the platform's z rotation
                gameManager.DisableSlowMotion(0); //Disable slow motion

                //Particle FX
                GameObject colFX = (GameObject)Instantiate(playerImpactParticles, colCast.point, Quaternion.LookRotation(colCast.normal)); //Instantiate a new collision effect
                Destroy(colFX, 0.5f); //Destroys the collision effect

                playerState.ChangeState(State.Stationary); //Change player back to the stationary state
            }
        }
    }

    //Collision with platform exit
    void HandleCollisionExit_Platform(Collider2D platformCollision)
    {
        //Removes colliding object from list
        if (collidingObjects.Contains(platformCollision.transform.gameObject))
        {
            collidingObjects.Remove(platformCollision.transform.gameObject);
        }
    }

    //Collision with enemy enter
    void HandleCollisionEnter_Enemy(Collider2D enemyCollision)
    {
        Die();
    }

    //Player Death
    void Die()
    {
        //Save high score
        if(levelManager.GetHighScoreMinutes() > PlayerPrefs.GetInt("HighScoreMinutes"))
            PlayerPrefs.SetInt("HighScoreMinutes", levelManager.GetHighScoreMinutes());

        if(levelManager.GetHighScoreSeconds() > PlayerPrefs.GetInt("HighScoreSeconds"))
            PlayerPrefs.SetInt("HighScoreSeconds", levelManager.GetHighScoreSeconds());

        PlayerPrefs.Save();

        //Kill player
        gameManager.DisableSlowMotion(0); //Disables slow mo
        GameObject colFX = (GameObject)Instantiate(playerDeathParticles, transform.position, Quaternion.identity); //Creates destruction effect
        Destroy(colFX, 2); //Destroy destruction effect in 2 seconds
        Destroy(this.gameObject); //Destroy the player object
        gameManager.LoadScene(SceneManager.GetActiveScene().name, 1); //Reload the scene in 1 second
    }

    //Returns true if the player can jump
    bool PlayerCanJump()
    {
        RaycastHit2D ray = Physics2D.Raycast(transform.position, inputManager.GetMouseDirectionFrom(this.gameObject), 50, LayerMask.GetMask("Platform"));

        //If jump point is the platform and the distance is too short, return false
        if(collidingObjects.Count > 0)
        {
            foreach(GameObject obj in collidingObjects)
            {
                if(ray.transform.gameObject == obj)
                {
                    return false;
                }
            }
        }
        return true;
    }

    //Forces player to jump from game start
    public void ForceJump(StartLevelFromGUI sg)
    {
        if(sg.gameObject.name == "BeginButton")
        {
            playerState.ChangeState(State.MovingNormal);
        }
        else
        {
            Debug.LogError("THIS METHOD SHOULD ONLY BE CALLED FROM THE BEGIN BUTTON OBJECT");
        }
    }

    //Updates the LineRenderer's positioning
    void UpdateLineRenderer()
    {
        //Update Positioning
        if (inputManager.GetMouseDistanceFrom(this.gameObject) >= lineRendererOffset) //If the mouse is not pointing at the player, render line normally
        {
            lineRenderer.SetPositions(new Vector3[] { 
            (Vector2)transform.position + new Vector2(-inputManager.GetMouseSinFrom(this.gameObject), inputManager.GetMouseCosFrom(this.gameObject)) * lineRendererOffset,
            inputManager.GetMousePosition()
        });
        }
        else //If the mouse is pointing at the player, render end point at the origin point
        {
            lineRenderer.SetPositions(new Vector3[] { 
            (Vector2)transform.position + new Vector2(-inputManager.GetMouseSinFrom(this.gameObject), inputManager.GetMouseCosFrom(this.gameObject)) * lineRendererOffset,
            (Vector2)transform.position + new Vector2(-inputManager.GetMouseSinFrom(this.gameObject), inputManager.GetMouseCosFrom(this.gameObject)) * lineRendererOffset
        });
        }

        //Update Colors
        if (PlayerCanJump())
        {
            shield.sprite = greenShield;
            glow.sprite = greenGlow;
            lineRenderer.SetColors(lineRendererNormalStart, lineRendererNormalEnd);
        }
        else
        {
            shield.sprite = redShield;
            glow.sprite = redGlow;
            lineRenderer.SetColors(lineRendererBlockedStart, lineRendererBlockedEnd);
        }
    }

    //Enable/Disable the line renderer
    void LineRendererEnabled(bool enabled)
    {
        switch(enabled){
            case true:
                lineRenderer.enabled = true;
                UpdateLineRenderer();
                break;
            case false:
                lineRenderer.enabled = false;
                break;
        }
    }
}
