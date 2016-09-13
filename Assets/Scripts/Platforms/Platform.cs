using UnityEngine;
using System.Collections;

public class Platform : MonoBehaviour {

    enum PlatformColor { Normal, Player };

    public string glowSortingLayer, mainSortingLayer;
    public Sprite blueGlowSprite, greenGlowSprite;
    public float glowOffset;

    float storedSliceWidth, storedSliceHeight;
    NineSlice spriteSlicer;
    NineSlice glowEffectSlicer;

    void Awake()
    {
        GetComponent<MeshRenderer>().sortingLayerID = SortingLayer.NameToID(mainSortingLayer);
        GetComponent<MeshRenderer>().sortingOrder = 0;
        GetComponentInChildren<MeshRenderer>().sortingLayerID = SortingLayer.NameToID(glowSortingLayer);
        GetComponent<MeshRenderer>().sortingOrder = 0;
        spriteSlicer = GetComponent<NineSlice>();
        storedSliceHeight = spriteSlicer.height;
        storedSliceWidth = spriteSlicer.width;
        glowEffectSlicer = transform.Find("GlowEffect").GetComponent<NineSlice>();
        glowEffectSlicer.width = spriteSlicer.width + glowOffset;
        glowEffectSlicer.height = spriteSlicer.height + glowOffset;
    }

    void LateUpdate()
    {
        //If the sprite slicer changes size, update the glow effect's size
        if (storedSliceWidth != spriteSlicer.width || storedSliceHeight != spriteSlicer.height)
        {
            glowEffectSlicer.width = spriteSlicer.width + glowOffset;
            glowEffectSlicer.height = spriteSlicer.height + glowOffset;
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
                glowEffectSlicer.useSprite = blueGlowSprite;
                break;
            case PlatformColor.Player:
                glowEffectSlicer.useSprite = greenGlowSprite;
                break;
        }
    }

}
