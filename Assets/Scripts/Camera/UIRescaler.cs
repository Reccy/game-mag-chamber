using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIRescaler : MonoBehaviour {

    GraphicsManager graphics;
    RectTransform panelTransform; //UI Panel Transform

	void Awake()
    {
        graphics = Object.FindObjectOfType<GraphicsManager>();
        panelTransform = (RectTransform)transform.GetChild(0).gameObject.transform;
    }

    void Start()
    {
        RescaleUI();
    }

    public void RescaleUI()
    {
        if (graphics.aspectRatio <= 1.25f) //If aspect ratio is less than 4:3
        {
            panelTransform.anchorMax = new Vector2(1, 0.97f);
            panelTransform.anchorMin = new Vector2(0, 0.03f);
        }
        else //Otherwise, reset camera
        {
            panelTransform.anchorMax = new Vector2(1, 1);
            panelTransform.anchorMin = new Vector2(0, 0);
        }
    }
}
