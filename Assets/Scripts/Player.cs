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
    public float lineRendererOffset = 0.20f;

    void Awake()
    {
        inputManager = Object.FindObjectOfType<InputManager>();
        lineRenderer = GetComponent<LineRenderer>();
        Debug.Log(lineRenderer);

        playerState = StateMachine<State>.Initialize(this); //Init state machine
        playerState.ChangeState(State.Stationary);
    }

    //Stationary State
    void Stationary_Enter()
    {
        lineRenderer.enabled = true;
    }

    void Stationary_Update()
    {
        UpdateLineRenderer();
    }

    //MovingNormal State
    void MovingNormal_Enter()
    {
        lineRenderer.enabled = false;
    }

    void MovingNormal_Update()
    {

    }

    //MovingFast State
    void MovingFast_Enter()
    {
        lineRenderer.enabled = false;
    }

    void MovingFast_Update()
    {

    }

    //MovingSlow State
    void MovingSlow_Enter()
    {
        lineRenderer.enabled = false;
    }

    void MovingSlow_Update()
    {

    }

    //MovingRedirected State
    void MovingRedirected_Enter()
    {
        lineRenderer.enabled = false;
    }

    void MovingRedirected_Update()
    {

    }

    //Other Methods

    //Updates the LineRenderer's positioning
    void UpdateLineRenderer()
    {
        lineRenderer.SetPositions(new Vector3[] { new Vector2(-inputManager.GetMouseSinFrom(this.gameObject), inputManager.GetMouseCosFrom(this.gameObject)) * lineRendererOffset, inputManager.GetMousePosition() });
    }
}
