using UnityEngine;
using System.Collections;

public class PlatformMovement_Test : MonoBehaviour {

    Vector2 startPos;

    void Awake()
    {
        startPos = transform.position;
    }

    void FixedUpdate()
    {
        transform.position = startPos + new Vector2(Mathf.Sin(Time.time), Mathf.Cos(Time.time));
    }
}
