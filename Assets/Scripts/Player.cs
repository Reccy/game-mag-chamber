using UnityEngine;
using System.Collections;
using MonsterLove.StateMachine;

public class Player : MonoBehaviour
{
    //Manager variables
    InputManager inputManager;

    //State machine variables
    public enum State { Stationary, MovingNormal, MovingFast, MovingSlow, MovingRedirected };
    StateMachine<State> playerState;

    //Other variables
    LineRenderer lineRenderer;

    Quaternion movingRotation; //Rotation for player's movement vector

    public float playerSpeed = 150;
    public float playerSpeedFast = 200;

    public float lineRendererOffset = 0.20f;

    void Awake()
    {
        inputManager = Object.FindObjectOfType<InputManager>();
        lineRenderer = GetComponent<LineRenderer>();

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
        transform.Translate(movingRotation * (Vector2.up * playerSpeed * Time.deltaTime));
    }

    //MovingFast State
    void MovingFast_Enter()
    {
        LineRendererEnabled(false);
        Time.timeScale = 1;
    }

    void MovingFast_FixedUpdate()
    {
        //Move forward
        transform.Translate(movingRotation * (Vector2.up * playerSpeedFast * Time.deltaTime));
    }

    //MovingSlow State
    void MovingSlow_Enter()
    {
        LineRendererEnabled(true);
        Time.timeScale = 0.5f;
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
        transform.Translate(movingRotation * (Vector2.up * playerSpeed * Time.deltaTime));
    }

    //MovingRedirected State
    void MovingRedirected_Enter()
    {
        LineRendererEnabled(false);
        Time.timeScale = 1;
        movingRotation = inputManager.GetMouseQuaternionFrom(this.gameObject);
    }

    void MovingRedirected_FixedUpdate()
    {
        //Move forward
        transform.Translate(movingRotation * (Vector2.up * playerSpeed * Time.deltaTime));
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
