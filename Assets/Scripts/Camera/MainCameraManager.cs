using UnityEngine;
using System.Collections;

public class MainCameraManager : MonoBehaviour {

    GraphicsManager graphics; //Graphics manager
    private Vector3 originPosition; //Original position of the camera
    CameraShake camShake;

    void Awake()
    {
        graphics = Object.FindObjectOfType<GraphicsManager>();
        camShake = GetComponent<CameraShake>();
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

    //Sets the screen shake paramaters
    public void ScreenShake(float duration = 0.2f)
    {
        camShake.shakeDuration = duration;
    }
}
