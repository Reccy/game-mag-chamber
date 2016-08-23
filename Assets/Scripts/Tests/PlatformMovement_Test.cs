using UnityEngine;
using System.Collections;

public class PlatformMovement_Test : MonoBehaviour {

    GameManager gameManager;
    Vector2 startPos;
    public bool spin = false;

    void Awake()
    {
        gameManager = Object.FindObjectOfType<GameManager>();
        startPos = transform.position;
    }

    void FixedUpdate()
    {
        transform.position = startPos + new Vector2(Mathf.Sin(Time.time), Mathf.Cos(Time.time));
        
        if(spin)
        {
            transform.Rotate(Vector3.forward * Time.deltaTime * gameManager.slowMotionMultiplier * 60f);
        }
    }
}
