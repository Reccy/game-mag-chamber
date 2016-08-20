using UnityEngine;
using System.Collections;
using MonsterLove.StateMachine;

public class SlowMo_Test : MonoBehaviour {

    public enum State { Normal, Modified, Slow };
    StateMachine<State> state;
    public State objectState = State.Normal;

    GameManager gameManager;

    void Awake()
    {
        gameManager = Object.FindObjectOfType<GameManager>();
        state = StateMachine<State>.Initialize(this);
        state.ChangeState(objectState);
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Q))
        {
            StartCoroutine(gameManager.EnableSlowMotion(0.5f));
        }

        if(Input.GetKeyDown(KeyCode.E))
        {
            StartCoroutine(gameManager.DisableSlowMotion(0.5f));
        }
    }

    void Normal_FixedUpdate()
    {
        transform.Translate(Vector2.up * 5f * Time.deltaTime);
    }

    void Modified_FixedUpdate()
    {
        if (transform.position.y > 0)
        {
            transform.Translate(Vector2.up * 5f * Time.deltaTime * gameManager.slowMotionTime);
        }
        else
        {
            transform.Translate(Vector2.up * 5f * Time.deltaTime);
        }
    }

    void Slow_FixedUpdate()
    {
        transform.Translate(Vector2.up * 5f * Time.deltaTime * gameManager.slowMotionTime);
    }
}
