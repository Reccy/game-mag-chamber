using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MonsterLove.StateMachine;

public class Enemy_Flanker : BulletPattern {

    private enum State { Squashing, Exploding }
    private StateMachine<State> state;

    private SoundManager sound;

    private List<GameObject> flankers;
    private Vector2 targetPosition;

    public GameObject explosionBullet;

    void Awake()
    {
        state = StateMachine<State>.Initialize(this);
        flankers = new List<GameObject>();
        sound = Object.FindObjectOfType<SoundManager>();

        foreach(Transform child in transform)
        {
            flankers.Add(child.gameObject);
        }

        Destroy(this.gameObject, 10);
    }

    void Start()
    {
        state.ChangeState(State.Squashing);
    }

    void Squashing_Enter()
    {
        if (Player)
            targetPosition = Player.transform.position;
        else
            state.ChangeState(State.Exploding);

        transform.position = targetPosition;
    }

    void Squashing_FixedUpdate()
    {
        if (Vector2.Distance(flankers[0].GetComponent<Enemy_FlankerBullet>().transform.position, targetPosition) < 0.3f)
        {
            state.ChangeState(State.Exploding);
        }
    }

    void Exploding_Enter()
    {
        sound.Play("sfx_BomberExplosion", SoundManager.SoundChannel.SFX, 0.3f, false, 0.3f);
        Camera.main.GetComponent<MainCameraManager>().ScreenShake(0.4f);
        GameObject g = Instantiate(explosionBullet, transform.position, Quaternion.identity) as GameObject;
        g.transform.parent = null;
        Destroy(g, 10);
        Destroy(this.gameObject);
    }
}
