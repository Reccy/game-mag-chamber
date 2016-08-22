using UnityEngine;
using System.Collections;

public class PhysicsBall_Test : MonoBehaviour {

	void FixedUpdate()
    {
        if(transform.position.y < -5)
        {
            Debug.Log("Collision Miss!");
        }
    }
}
