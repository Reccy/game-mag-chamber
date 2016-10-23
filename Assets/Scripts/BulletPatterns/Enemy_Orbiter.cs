using UnityEngine;
using UnityExtensions.MathfExtensions;
using System.Collections;
using MonsterLove.StateMachine;

public class Enemy_Orbiter : BulletPattern {

    private enum State { Seeking, Orbiting, PreLaunch, Launch } //Orbiter's current state
    private StateMachine<State> state;

    private float seekingSpeed = 10f; //Speed while seeking
    private float orbitSpeed = 80f; //Speed at which the orbiter rotates
    private float anticipationSpeed = 5f; //Speed at which the orbiter pulls back in anticipation of launching
    private float launchSpeed = 40f; //Speed at launch

    private float orbitDistance = 3f; //Distance to orbit the player at
    private float orbitingTimer = 4f; //Duration of orbiting
    private float preLaunchTimer = 0.5f; //Duration of PreLaunch state

    void Awake()
    {
        state = StateMachine<State>.Initialize(this);
        state.ChangeState(State.Seeking);
    }

    //Orbiter is seeking the player
    void Seeking_FixedUpdate()
    {
        //Move towards the player
        transform.Translate(Vector2.up * seekingSpeed * GameManager.slowMotionMultiplier * Time.deltaTime);

        if(Player)
        {
            float angleToPlayer = MathfExtensions.AngleFromTo(this.gameObject, Player);
            transform.rotation = Quaternion.AngleAxis(angleToPlayer, Vector3.forward);

            //Check if ready to orbit
            if (Vector2.Distance(transform.position, Player.transform.position) < orbitDistance)
            {
                state.ChangeState(State.Orbiting);
            }
        }
        else
        {
            state.ChangeState(State.PreLaunch);
        }
    }

    //Orbiter is... ORBITING!
    void Orbiting_FixedUpdate()
    {
        if(Player)
        {
            //Rotate around the player
            transform.RotateAround(Player.transform.position, Vector3.forward, orbitSpeed * Time.deltaTime);
            Vector3 desiredPosition = (transform.position - Player.transform.position).normalized * orbitDistance + Player.transform.position;
            transform.position = Vector3.MoveTowards(transform.position, desiredPosition, Time.deltaTime * orbitSpeed);
            float angleToPlayer = MathfExtensions.AngleFromTo(this.gameObject, Player);
            transform.rotation = Quaternion.AngleAxis(angleToPlayer, Vector3.forward);

            //Check if it's time for launch
            if (orbitingTimer < 0)
                state.ChangeState(State.PreLaunch);
            else
                orbitingTimer -= (GameManager.slowMotionMultiplier * Time.deltaTime);
        }
        else
        {
            state.ChangeState(State.PreLaunch);
        }
    }

    //Orbiter is about to launch (anticipation animation)
    void PreLaunch_FixedUpdate()
    {
        //Pull back
        anticipationSpeed -= 10 * GameManager.slowMotionMultiplier * Time.deltaTime;

        //Move backwards
        transform.Translate(-Vector2.up * anticipationSpeed * GameManager.slowMotionMultiplier * Time.deltaTime);

        //Check if it's time to finally launch
        if(anticipationSpeed <= 0)
        {
            anticipationSpeed = 0;
            preLaunchTimer -= GameManager.slowMotionMultiplier * Time.deltaTime;

            //COME ON, LAUNCH! >:O
            if (preLaunchTimer <= 0)
                state.ChangeState(State.Launch);
        }
    }

    //Orbiter is launching
    void Launch_FixedUpdate()
    {
        //Move forward very quickly
        transform.Translate(Vector2.up * launchSpeed * GameManager.slowMotionMultiplier * Time.deltaTime);

        //Destroy when orbiter goes out of bounds
        if (transform.position.x < -20 || transform.position.x > 20 || transform.position.y < -20 || transform.position.y > 20)
        {
            Destroy(this.gameObject);
        }
    }
}
