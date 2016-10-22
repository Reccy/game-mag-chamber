using UnityEngine;
using UnityExtensions.MathfExtensions;
using System.Collections;

public class Enemy_CDuster : BulletPattern {

    private float speed = 6f; //Speed of cduster
    private float shootLimit = 1.34f; //Duration between shots
    private float originalShootLimit; //Original duration between shots
    public GameObject bullet; //Bullet to be instantiated

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

    void FixedUpdate()
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
