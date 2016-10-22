using UnityEngine;
using UnityExtensions.MathfExtensions;
using System.Collections;
using MonsterLove.StateMachine;

public class Enemy_Pointer : BulletPattern
{
    private enum State { Seeking, Shooting }
    StateMachine<State> state;
    private float timeUntilLaunch = 5f;

    void Awake()
    {
        state = StateMachine<State>.Initialize(this);
        state.ChangeState(State.Seeking);
    }

    void Seeking_Update()
    {
        //Move forwards
        transform.Translate(Vector2.up * 3 * GameManager.slowMotionMultiplier * Time.deltaTime);

        //Rotate towards player
        if(Player)
        {
            float angleToPlayer = MathfExtensions.AngleFromTo(this.gameObject, Player);
            transform.rotation = Quaternion.AngleAxis(angleToPlayer, Vector3.forward);

            //Check if ready to shoot
            if (timeUntilLaunch <= 0 || Vector2.Distance(transform.position, Player.transform.position) < 2.5f)
            {
                state.ChangeState(State.Shooting);
            }

            timeUntilLaunch -= (GameManager.slowMotionMultiplier * Time.deltaTime);
        }
        else
        {
            state.ChangeState(State.Shooting);
        }
        
    }

    //Move forward
    void Shooting_Update()
    {
        transform.Translate(Vector2.up * 10 * GameManager.slowMotionMultiplier * Time.deltaTime);
    }

    void Update()
    {
        //Destroy when obstacle goes out of bounds
        if (transform.position.x < -20 || transform.position.x > 20 || transform.position.y < -20 || transform.position.y > 20)
        {
            Destroy(this.gameObject);
        }
    }
}
