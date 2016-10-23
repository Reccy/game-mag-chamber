using UnityEngine;
using UnityExtensions.MathfExtensions;
using System.Collections;
using MonsterLove.StateMachine;

public class Enemy_Pointer : BulletPattern
{
    private enum State { Anticipation, Seeking, Shooting }
    private StateMachine<State> state;
    private float anticipationTimer = 0.5f;
    private float anticipationSpeed;
    private float endTimer = 5f;
    private float speed = 3;

    void Awake()
    {
        state = StateMachine<State>.Initialize(this);
        state.ChangeState(State.Anticipation);
    }

    void Start()
    {
        if (Player)
        {
            float angleToPlayer = MathfExtensions.AngleFromTo(this.gameObject, Player);
            transform.rotation = Quaternion.AngleAxis(angleToPlayer, Vector3.forward);
            transform.Rotate(0, 0, transform.rotation.z + Random.Range(-5, 5));
        }
    }

    void Anticipation_Enter()
    {
        anticipationSpeed = speed;
    }

    void Anticipation_FixedUpdate()
    {
        //Slow down once object enters the game boundaries
        if ((transform.position.x > -9.4f && transform.position.x < 9.4f) && (transform.position.y > -7.3f && transform.position.y < 7.3f))
        {
            if(anticipationSpeed <= 0)
            {
                anticipationSpeed = 0;
                if(anticipationTimer < 0)
                {
                    state.ChangeState(State.Seeking);
                }
                else
                {
                    anticipationTimer -= GameManager.slowMotionMultiplier * Time.deltaTime;
                }
            }
            else
            {
                anticipationSpeed -= 0.05f * GameManager.slowMotionMultiplier;
            }
        }

        //Move forwards
        transform.Translate(Vector2.up * anticipationSpeed * GameManager.slowMotionMultiplier * Time.deltaTime);

        Debug.Log("Anticipation Speed: " + anticipationSpeed + ", Anticipation Timer: " + anticipationTimer + ", Transform Position: " + transform.position);
    }

    void Seeking_FixedUpdate()
    {
        //Move forwards
        transform.Translate(Vector2.up * speed * GameManager.slowMotionMultiplier * Time.deltaTime);

        //Rotate towards player
        if(Player)
        {
            float angleToPlayer = MathfExtensions.AngleFromTo(this.gameObject, Player);
            transform.rotation = Quaternion.AngleAxis(angleToPlayer, Vector3.forward);

            //Check if ready to shoot
            if (endTimer <= 0 || Vector2.Distance(transform.position, Player.transform.position) < 3.5f)
            {
                state.ChangeState(State.Shooting);
            }

            endTimer -= (GameManager.slowMotionMultiplier * Time.deltaTime);
        }
        else
        {
            state.ChangeState(State.Shooting);
        }
        
    }

    //Move forward
    void Shooting_FixedUpdate()
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
