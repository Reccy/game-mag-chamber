using UnityEngine;
using UnityExtensions.MathfExtensions;
using System.Collections;
using MonsterLove.StateMachine;

public class Enemy_CDuster : BulletPattern {

    private enum State { Anticipation, Moving };
    private StateMachine<State> state;

    private float speed = 6f; //Speed of cduster
    private float anticipationSpeed = 3f; //Speed during anticipation state
    private float shootLimit = 0.6f; //Duration between shots
    private float originalShootLimit; //Original duration between shots
    private float anticipationTimer = 0.5f; //Duration of anticipation state
    public GameObject bullet; //Bullet to be instantiated

    void Awake()
    {
        state = StateMachine<State>.Initialize(this);
        state.ChangeState(State.Anticipation);
    }

    void Start()
    {
        if(Player)
        {
            float angleToPlayer = MathfExtensions.AngleFromTo(this.gameObject, Player);
            transform.rotation = Quaternion.AngleAxis(angleToPlayer, Vector3.forward);
            transform.Rotate(0, 0, transform.rotation.z + Random.Range(-5, 5));
        }

        originalShootLimit = shootLimit;
    }

    void Anticipation_FixedUpdate()
    {
        //Slow down once object enters the game boundaries
        if ((transform.position.x > -9.4f && transform.position.x < 9.4f) && (transform.position.y > -7.3f && transform.position.y < 7.3f))
        {
            if (anticipationSpeed <= 0)
            {
                anticipationSpeed = 0;
                if (anticipationTimer < 0)
                {
                    state.ChangeState(State.Moving);
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
    }

    void Moving_FixedUpdate()
    {
        //Move forward very quickly
        transform.Translate(Vector2.up * speed * GameManager.slowMotionMultiplier * Time.deltaTime);

        shootLimit -= GameManager.slowMotionMultiplier * Time.deltaTime;

        if(shootLimit < 0)
        {
            shootLimit = originalShootLimit;
            Instantiate(bullet, transform.position, transform.rotation);
        }

        //Destroy when cduster goes out of bounds
        if (transform.position.x < -20 || transform.position.x > 20 || transform.position.y < -20 || transform.position.y > 20)
        {
            Destroy(this.gameObject);
        }
    }
}
