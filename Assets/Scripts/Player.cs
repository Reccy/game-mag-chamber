using UnityEngine;
using System.Collections;
using MonsterLove.StateMachine;
using UnityExtensions.Physics2DExtensions;

public class Player : MonoBehaviour
{
    //Manager variables
    InputManager inputManager;
    GameManager gameManager;

    //State machine variables
    public enum State { Stationary, MovingNormal, MovingFast, MovingSlow, MovingRedirected };
    StateMachine<State> playerState;

    //Other variables
    LineRenderer lineRenderer;
    CircleCollider2D col;

    //Platform variables
    GameObject platform;
    Vector2 platformOffset;
    Quaternion platformRotation;

    Quaternion movingRotation; //Rotation for player's movement vector

    public float playerSpeed = 150;
    public float playerSpeedFast = 200;
    public float lineRendererOffset = 0.20f;

    float collisionRadius; //Offset for manual collision detection

    void Awake()
    {
        inputManager = Object.FindObjectOfType<InputManager>();
        gameManager = Object.FindObjectOfType<GameManager>();
        lineRenderer = GetComponent<LineRenderer>();
        col = GetComponent<CircleCollider2D>();
        collisionRadius = col.radius + 0.01f;

        playerState = StateMachine<State>.Initialize(this); //Init state machine
        playerState.ChangeState(State.Stationary);
    }

    //Stationary State
    void Stationary_Enter()
    {
        LineRendererEnabled(true);
    }

    void Stationary_Update()
    {
        //Handle Input
        if(Input.GetMouseButtonDown(0)) //LMB Down -> Moving Normal
        {
            playerState.ChangeState(State.MovingNormal);
        }
        UpdateLineRenderer();
    }

    void Stationary_FixedUpdate()
    {
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
    }

    void Stationary_Finally()
    {
        platform = null;
    }

    //MovingNormal State
    void MovingNormal_Enter()
    {
        LineRendererEnabled(false);
        movingRotation = inputManager.GetMouseQuaternionFrom(this.gameObject);
    }

    void MovingNormal_Update()
    {
        //Handle Input
        if(Input.GetMouseButtonDown(0)) //LMB Down -> Moving Slow
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
        if(Input.GetMouseButtonUp(0)) //LMB Up -> Moving Redirected
        {
            playerState.ChangeState(State.MovingRedirected);
        }
        else if(Input.GetMouseButtonDown(1)) //RMB Down -> Moving Fast
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

        if(colCast.transform.gameObject != platform)
        {
            platform = colCast.transform.gameObject; //Set the connected platform
            transform.position = colCast.point + (colCast.normal * col.radius); //Set the player's position to the platform surface
            platformOffset = transform.position - platform.transform.position; //Get the offset relative to the platform's center
            platformRotation = platform.transform.rotation; //Get the platform's z rotation
            gameManager.DisableSlowMotion(0); //Disable slow motion

            playerState.ChangeState(State.Stationary);
        }
    }

    //Line Renderer Methods
    //Updates the LineRenderer's positioning
    void UpdateLineRenderer()
    {
        if(inputManager.GetMouseDistanceFrom(this.gameObject) >= lineRendererOffset) //If the mouse is not pointing at the player, render line normally
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
