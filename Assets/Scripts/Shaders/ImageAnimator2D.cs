using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ImageAnimator2D : MonoBehaviour
{
    private RawImage img;
    public float speed = 0.17f;

    void Awake()
    {
        img = GetComponent<RawImage>();
    }

    void Update()
    {
        float offset = Time.time * speed;
        img.uvRect = new Rect(offset, offset/1.2f, img.uvRect.width, img.uvRect.height);
    }
}
