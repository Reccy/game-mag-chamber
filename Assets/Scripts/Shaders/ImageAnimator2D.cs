using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ImageAnimator2D : MonoBehaviour
{
    private RawImage img;

    void Awake()
    {
        img = GetComponent<RawImage>();
    }

    void Update()
    {
        float offset = Time.time * 0.17f;
        img.uvRect = new Rect(offset, offset/1.2f, img.uvRect.width, img.uvRect.height);
    }
}
