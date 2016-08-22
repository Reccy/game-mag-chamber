using UnityEngine;
using System.Collections;

public class PlatformMovement_Test : MonoBehaviour {

    Vector2 startPos;
    public bool spin = false;

    void Awake()
    {
        startPos = transform.position;
    }

    void FixedUpdate()
    {
        transform.position = startPos + new Vector2(Mathf.Sin(Time.time), Mathf.Cos(Time.time));
        
        if(spin)
        {
            transform.Rotate(Vector3.forward * Time.deltaTime * 60f);
        }
    }
}
