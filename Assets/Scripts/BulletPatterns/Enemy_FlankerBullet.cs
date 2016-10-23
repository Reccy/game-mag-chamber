using UnityEngine;
using System.Collections;

public class Enemy_FlankerBullet : MonoBehaviour {

    private float speed = 8;
    private GameManager gm;

    void Awake()
    {
        gm = Object.FindObjectOfType<GameManager>();
    }

    void Update()
    {
        //Move forward
        transform.Translate(Vector2.up * speed * gm.slowMotionMultiplier * Time.deltaTime);
    }

}
