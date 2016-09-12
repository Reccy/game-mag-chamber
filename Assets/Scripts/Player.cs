using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using MonsterLove.StateMachine;
using UnityExtensions.Physics2DExtensions;

public class Player : MonoBehaviour
{
    //Manager variables
    InputManager inputManager;
    GameManager gameManager;
    MainCameraManager cameraManager;
    SoundManager sound;

    //State machine variables
    public enum State { Stationary, MovingNormal, MovingFast, MovingSlow, MovingRedirected };
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

    //Platform variables
    GameObject platform;
    Vector2 platformOffset;
    Quaternion platformRotation;

    Quaternion movingRotation; //Rotation for player's movement vector

    public float generatorRotationSpeed = 50;
    public float playerSpeed = 30;
    public float playerSpeedFast = 45;
    public float lineRendererOffset = 0.20f;

    float generatorRotationPercent = 1;
    float collisionRadius; //Offset for manual collision detection

    void Awake()
    {
        inputManager = Object.FindObjectOfType<InputManager>();
        gameManager = Object.FindObjectOfType<GameManager>();
        cameraManager = Object.FindObjectOfType<MainCameraManager>();
        sound = Object.FindObjectOfType<SoundManager>();
        shield = transform.Find("Shield").GetComponent<SpriteRenderer>();
        glow = transform.Find("Glow").GetComponent<SpriteRenderer>();
        lineRenderer = GetComponent<LineRenderer>();
        col = GetComponent<CircleCollider2D>();
        generator = transform.Find("Generator").gameObject;
        eye = transform.Find("Eye").gameObject;
        collisionRadius = col.radius + 0.01f;
        lineRenderer.sortingLayerName = "Game";

        playerState = StateMachine<State>.Initialize(this); //Init state machine
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

        //Check if player is within game's bounds
        if(transform.position.x < -6.5 || transform.position.x > 6.5 || transform.position.y < -5 || transform.position.y > 5)
        {
            Die();
        }
    }

    //Stationary State
    void Stationary_Enter()
    {
        LineRendererEnabled(true);
        sound.PlayOneShot("sfx_Land", SoundManager.SoundChannel.SFX, 0.8f, false, 0.3f, 128);
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
        LineRendererEnabled(false);
        movingRotation = inputManager.GetMouseQuaternionFrom(this.gameObject);
        sound.PlayOneShot("sfx_Jump", SoundManager.SoundChannel.SFX, 0.9f, false, 0.3f, 128);
    }

    void MovingNormal_Update()
    {
        //Handle Input
        if(inputManager.GetJumpButtonDown()) //LMB Down -> Moving Slow
        {
            playerState.ChangeState(State.MovingSlow);
        }
    }

    void MovingNormal_FixedUpdate()
    {
        //Move forward
        transform.Translate(movingRotation * (Vector2.up * playerSpeed * gameManager.slowMotionMultiplier * Time.deltaTime));
    }

    //MovingFast State
    void MovingFast_Enter()
    {
        LineRendererEnabled(false);
        gameManager.DisableSlowMotion(0);
        generatorRotationPercent = 60;
    }

    void MovingFast_FixedUpdate()
    {
        //Move forward
        transform.Translate(movingRotation * (Vector2.up * playerSpeedFast * gameManager.slowMotionMultiplier * Time.deltaTime));
    }

    //MovingSlow State
    void MovingSlow_Enter()
    {
        LineRendererEnabled(true);
        gameManager.EnableSlowMotion(0.1f);
    }

    void MovingSlow_Update()
    {
        //Manage Input
        if(inputManager.GetJumpButtonUp()) //LMB Up -> Moving Redirected
        {
            playerState.ChangeState(State.MovingRedirected);
        }
        else if(inputManager.GetBoostButtonDown()) //RMB Down -> Moving Fast
        {
            playerState.ChangeState(State.MovingFast);
        }

        //Update Line Renderer
        UpdateLineRenderer();
    }

    void MovingSlow_FixedUpdate()
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
        transform.Translate(movingRotation * (Vector2.up * playerSpeed * gameManager.slowMotionMultiplier * Time.deltaTime));
    }

    //Collision Detection Code
    //Trigger Enter Switch
    void OnTriggerEnter2D(Collider2D colObj)
    {
        string colLayer = LayerMask.LayerToName(colObj.gameObject.layer);

        switch (colLayer)
        {
            case "Platform":
                HandleCollision_Platform(colObj);
                break;
        }
    }

    //Collision with platform
    void HandleCollision_Platform(Collider2D platformCollision)
    {
        RaycastHit2D colCast = Physics2DExtensions.ArcCast(transform.position, 0, 360, 360, collisionRadius, LayerMask.GetMask("Platform"));

        if(colCast)
        {
            if (colCast.transform.gameObject != platform) //If the player collides with a different platform
            {
                cameraManager.ScreenShake(0.2f); //Screen shake

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

    //Player Death
    void Die()
    {
        gameManager.DisableSlowMotion(0);
        GameObject colFX = (GameObject)Instantiate(playerDeathParticles, transform.position, Quaternion.identity);
        Destroy(colFX, 10);
        Destroy(this.gameObject);
        gameManager.LoadScene(SceneManager.GetActiveScene().name, 5);
    }

    //Line Renderer Methods
    bool PlayerCanJump()
    {
        RaycastHit2D ray = Physics2D.Raycast(transform.position, inputManager.GetMouseDirectionFrom(this.gameObject), 50, LayerMask.GetMask("Platform"));

        if(ray && platform)
        {
            if(ray.transform.gameObject == platform)
            {
                return false;
            }
        }
        return true;
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
