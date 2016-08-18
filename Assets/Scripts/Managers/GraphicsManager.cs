using UnityEngine;
using System.Collections;

public class GraphicsManager : MonoBehaviour {

    public float aspectRatio;
    public bool resetResolution;

    void Start()
    {
        if(resetResolution)
        {
            Screen.SetResolution(1024, 768, false);
        }
    }

    public void UpdateAspectRatio(float newAspectRatio)
    {
        aspectRatio = newAspectRatio;
        if (Camera.main != null)
        {
            GameObject.FindGameObjectWithTag("MainCanvas").GetComponent<UIRescaler>().RescaleUI();
            Camera.main.GetComponent<AspectRatioLetterboxer>().UpdateAspectRatio();
        }
    }
}
