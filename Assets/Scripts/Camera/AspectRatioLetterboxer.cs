using UnityEngine;
using System.Collections;

public class AspectRatioLetterboxer : MonoBehaviour {

    private float aspectRatio; //Current game resolution
    private Camera cam; //Attached Camera
    private GameObject letterboxTop, letterboxBottom;

	void Start () {
        cam = GetComponent<Camera>();
        aspectRatio = cam.aspect;
        letterboxTop = transform.Find("LetterboxTop").gameObject;
        letterboxBottom = transform.Find("LetterboxBottom").gameObject;
        UpdateAspectRatio();
	}
	
	void Update () {
        if (aspectRatio != cam.aspect) //If the resolution changes, update the aspect ratio.
        {
            UpdateAspectRatio();
        }
	}

    void UpdateAspectRatio()
    {
        aspectRatio = cam.aspect;

        if (aspectRatio <= 1.25f) //If aspect ratio is less than 4:3
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
