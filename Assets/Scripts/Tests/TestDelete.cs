using UnityEngine;
using System.Collections;

public class TestDelete : BulletPattern {

	void Start()
    {
        Destroy(this.gameObject, 1);
    }
}
