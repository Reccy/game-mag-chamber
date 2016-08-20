using UnityEngine;
using System.Collections;
using MonsterLove.StateMachine;

public class Framerate_Test : MonoBehaviour {

    private enum State { Moving, Stopped };
    StateMachine<State> objectState;

    public enum UpdateType { Update, FixedUpdate };
    public UpdateType uT;

    void Awake()
    {
        objectState = StateMachine<State>.Initialize(this);
        objectState.ChangeState(State.Moving);
    }

    void Moving_Enter()
    {
        Debug.Log("Test Object has entered 'Moving' state");
    }

    void Moving_Update()
    {
        if(uT == UpdateType.Update)
        {
            //Move forward
            transform.Translate(Vector2.up * 5f * Time.deltaTime);
            if(transform.position.y > 4 && transform.position.y < 4.07)
            {
                Debug.Log(transform.name + " reached goal at: " + Time.timeSinceLevelLoad);
                objectState.ChangeState(State.Stopped);
            }
        }
    }

    void Moving_FixedUpdate()
    {
        if(uT == UpdateType.FixedUpdate)
        {
            //Move forward
            transform.Translate(Vector2.up * 5f * Time.deltaTime);
            if (transform.position.y > 4 && transform.position.y < 4.07)
            {
                Debug.Log(transform.name + " reached goal at: " + Time.timeSinceLevelLoad);
                objectState.ChangeState(State.Stopped);
            }
        }
    }

    void Stopped_Enter()
    {
        /*
        transform.position = new Vector3(
            transform.position.x,
            4,
            transform.position.z
            );*/
    }
}
