using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class AspectRatioLetterboxer : MonoBehaviour {

    GraphicsManager graphics; //Graphics Manager

    Camera cam; //Attached Camera
    float originCamSize; //Original camera size

    void Awake()
    {
        graphics = Object.FindObjectOfType<GraphicsManager>();

        cam = GetComponent<Camera>();
        originCamSize = cam.orthographicSize;
    }

    public void UpdateAspectRatio()
    {
        if (graphics.aspectRatio <= 1.25f && SceneManager.GetActiveScene().name != "Level1") //If aspect ratio is less than 4:3
        {
            cam.orthographicSize = originCamSize * 1.07f;
        }
        else //Otherwise, reset camera
        {
            cam.orthographicSize = originCamSize;
        }
    }
}
