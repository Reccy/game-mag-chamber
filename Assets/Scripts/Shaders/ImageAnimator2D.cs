using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ImageAnimator2D : MonoBehaviour
{
    private RawImage img;
    public float speed = 0.17f;
    public bool scrollSkewed = true;

    void Awake()
    {
        img = GetComponent<RawImage>();
    }

    void Update()
    {
        float offset = Time.time * speed;
        if(scrollSkewed)
            img.uvRect = new Rect(offset, offset/1.2f, img.uvRect.width, img.uvRect.height);
        else
            img.uvRect = new Rect(0, offset, img.uvRect.width, img.uvRect.height);
    }
}
