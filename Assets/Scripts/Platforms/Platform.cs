using UnityEngine;
using System.Collections;

public class Platform : MonoBehaviour {

    enum PlatformColor { Normal, Player };

    public Sprite blueSprite, greenSprite, blueGlowSprite, greenGlowSprite;

    float storedSliceWidth, storedSliceHeight;
    NineSlice spriteSlicer;
    NineSlice glowEffectSlicer;

    void Awake()
    {
        spriteSlicer = GetComponent<NineSlice>();
        storedSliceHeight = spriteSlicer.height;
        storedSliceWidth = spriteSlicer.width;
        glowEffectSlicer = transform.Find("GlowEffect").GetComponent<NineSlice>();
        glowEffectSlicer.width = spriteSlicer.width + 0.1f;
        glowEffectSlicer.height = spriteSlicer.height + 0.1f;
    }

    void LateUpdate()
    {
        //If the sprite slicer changes size, update the glow effect's size
        if (storedSliceWidth != spriteSlicer.width || storedSliceHeight != spriteSlicer.height)
        {
            glowEffectSlicer.width = spriteSlicer.width + 0.1f;
            glowEffectSlicer.height = spriteSlicer.height + 0.1f;
            storedSliceWidth = spriteSlicer.width;
            storedSliceHeight = spriteSlicer.height;
        }
            
    }

    //Collision methods
    void OnTriggerEnter2D(Collider2D colObj)
    {
        string colLayer = LayerMask.LayerToName(colObj.gameObject.layer);

        switch(colLayer)
        {
            case "Player":
                SetColor(PlatformColor.Player);
                break;
        }
    }

    void OnTriggerExit2D(Collider2D colObj)
    {
        string colLayer = LayerMask.LayerToName(colObj.gameObject.layer);

        switch (colLayer)
        {
            case "Player":
                SetColor(PlatformColor.Normal);
                break;
        }
    }

    //Sets the platform's color by changing the glow sprite and platform sprite
    void SetColor(PlatformColor gc)
    {
        switch(gc)
        {
            case PlatformColor.Normal:
                spriteSlicer.useSprite = blueSprite;
                glowEffectSlicer.useSprite = blueGlowSprite;
                break;
            case PlatformColor.Player:
                spriteSlicer.useSprite = greenSprite;
                glowEffectSlicer.useSprite = greenGlowSprite;
                break;
        }
    }

}
