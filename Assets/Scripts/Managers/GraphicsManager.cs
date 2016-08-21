using UnityEngine;
using System.Collections;

public class GraphicsManager : MonoBehaviour {

    public float aspectRatio;
    public bool resetResolution;

    void Start()
    {
        //Fix Graphics Settings lag in the editor
        if (Application.isEditor)
        {
            QualitySettings.antiAliasing = 0;
            QualitySettings.vSyncCount = 0;
        }

        //Reset the resolution if enabled
        if(resetResolution)
        {
            Screen.SetResolution(1024, 768, false);
        }
    }

    //Updates aspect ratio on main camera
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
