using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LevelImageTransition : MonoBehaviour 
{
    public bool intro = true;
    RectTransform img;

    void Awake()
    {
        img = this.gameObject.GetComponent<RectTransform>();
        img.anchoredPosition = intro ? new Vector2(-100, 0) : new Vector2(-1600, 0);
    }

    void FixedUpdate()
    {
        float v = 0;

        if (intro)
        {
            img.anchoredPosition = new Vector2(Mathf.SmoothDamp(img.anchoredPosition.x, 1500, ref v, 0.02f), 0);
        }
        else
        {
            img.anchoredPosition = new Vector2(Mathf.SmoothDamp(img.anchoredPosition.x, 0, ref v, 0.02f) , 0);
        }
    }
}
