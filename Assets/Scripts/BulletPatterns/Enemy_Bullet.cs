using UnityEngine;
using System.Collections;

public class Enemy_Bullet : MonoBehaviour {

    private GameManager gm;

    void Awake()
    {
        gm = Object.FindObjectOfType<GameManager>();
    }

    void FixedUpdate()
    {
        transform.Translate(Vector2.up * 10 * gm.slowMotionMultiplier * Time.deltaTime);

        //Destroy when obstacle goes out of bounds
        if (transform.position.x < -20 || transform.position.x > 20 || transform.position.y < -20 || transform.position.y > 20)
        {
            Destroy(this.gameObject);
        }
    }

}
