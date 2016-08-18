using UnityEngine;
using System.Collections;

public class AspectRatioLetterboxer : MonoBehaviour {

    GraphicsManager graphics; //Graphics Manager

    Camera cam; //Attached Camera
    GameObject letterboxTop, letterboxBottom; //Letterboxes

    void Awake()
    {
        graphics = Object.FindObjectOfType<GraphicsManager>();

        cam = GetComponent<Camera>();
        letterboxTop = transform.Find("LetterboxTop").gameObject;
        letterboxBottom = transform.Find("LetterboxBottom").gameObject;
    }

    public void UpdateAspectRatio()
    {
        if (graphics.aspectRatio <= 1.25f) //If aspect ratio is less than 4:3
        {
            cam.orthographicSize = 5.35f;
            EnableLetterbox();
        }
        else //Otherwise, reset camera
        {
            cam.orthographicSize = 5f;
            DisableLetterbox();
        }
    }

    //Letterbox toggles
    void EnableLetterbox()
    {
        letterboxTop.SetActive(true);
        letterboxBottom.SetActive(true);
    }

    void DisableLetterbox()
    {
        letterboxTop.SetActive(false);
        letterboxBottom.SetActive(false);
    }
}
