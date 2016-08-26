using UnityEngine;
using System.Collections;
using MonsterLove.StateMachine;

public class SceneTransition : MonoBehaviour {

    public float speed = 20;

    enum State { Entering, Exiting, EnteringIdle, ExitingIdle }; //Entering = Entering a level. Exiting = Exiting a level.
    StateMachine<State> animState;

    GameObject topSprites, bottomSprites;

    public void Transition()
    {
        switch (animState.State)
        {
            case State.EnteringIdle:
                animState.ChangeState(State.Exiting);
                break;
            case State.ExitingIdle:
                animState.ChangeState(State.Entering);
                break;
        }
    }

    void Awake()
    {
        topSprites = transform.Find("TopSprites").gameObject;
        bottomSprites = transform.Find("BottomSprites").gameObject;

        animState = StateMachine<State>.Initialize(this);
        animState.ChangeState(State.ExitingIdle);
    }

    void Entering_FixedUpdate()
    {
        if(topSprites.transform.position.y < 10)
        {
            topSprites.transform.Translate(Vector2.up * speed * Time.deltaTime);
            bottomSprites.transform.Translate(Vector2.down * speed * Time.deltaTime);
        }
        else
        {
            topSprites.transform.position = new Vector2(0, 10);
            bottomSprites.transform.position = new Vector2(0, -10);
            animState.ChangeState(State.EnteringIdle);
        }
    }

    void Exiting_FixedUpdate()
    {
        if (topSprites.transform.position.y > 0)
        {
            topSprites.transform.Translate(Vector2.down * speed * Time.deltaTime);
            bottomSprites.transform.Translate(Vector2.up * speed * Time.deltaTime);
        }
        else
        {
            topSprites.transform.position = new Vector2(0, 0);
            bottomSprites.transform.position = new Vector2(0, -0);
            animState.ChangeState(State.ExitingIdle);
        }
    }

    public bool IsTransitioning()
    {
        if(animState.State == State.Entering || animState.State == State.Exiting)
        {
            return true;
        }
        return false;
    }
}
