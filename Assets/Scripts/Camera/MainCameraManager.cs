using UnityEngine;
using System.Collections;

public class MainCameraManager : MonoBehaviour {

    GraphicsManager graphics; //Graphics manager

    void Awake()
    {
        graphics = Object.FindObjectOfType<GraphicsManager>();
    }

	void Start () {
        graphics.UpdateAspectRatio(Camera.main.aspect);
	}
	
	void Update () {
        if(graphics.aspectRatio != Camera.main.aspect)
        {
            graphics.UpdateAspectRatio(Camera.main.aspect);
        }
	}
}
