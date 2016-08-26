using UnityEngine;
using System.Collections;

public class CanvasConnector : MonoBehaviour {

	void Awake()
    {
        GetComponent<Canvas>().worldCamera = Camera.main;
    }
}
