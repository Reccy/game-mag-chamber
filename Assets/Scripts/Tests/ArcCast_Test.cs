using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityExtensions.Physics2DExtensions;

public class ArcCast_Test : MonoBehaviour {

    int s, e;

    public void UpdateStart(InputField t)
    {
        int.TryParse(t.text, out s);
    }

    public void UpdateEnd(InputField t)
    {
        int.TryParse(t.text, out e);
    }

    public void Cast()
    {
        ArcCast(transform.position, s, e);
    }

    //Gets raycast around an arc
    RaycastHit2D ArcCast(Vector2 origin, int arcStart = 0, int arcRange = 360, int segments = 360, float radius = Mathf.Infinity, int layerMask = Physics2D.DefaultRaycastLayers)
    {
        List<RaycastHit2D> rayHitsList = new List<RaycastHit2D>();

        //Ensure segments don't go out of bounds
        if (segments > (arcRange - arcStart))
            segments = arcRange;

        //For loop to check every segment
        for (int i = 0; i < segments + 1; i++)
        {
            //Ensures deg0 and deg360 aren't cast twice
            if (i == 360)
            {
                break;
            }

            float angle = ((arcRange / segments) * i) + arcStart;
            Vector2 dir = Quaternion.Euler(0, 0, angle) * Vector2.up;

            RaycastHit2D rayHit = Physics2DExtensions.GetClosestRaycastHit2D(origin, dir, radius, layerMask);
            rayHitsList.Add(rayHit);

            if (i == 0)
            {
                Debug.DrawRay(origin, dir * 100, Color.HSVToRGB(0, 1, 1), 10, false);
            }
            else
            {
                Debug.DrawRay(origin, dir * 100, Color.HSVToRGB((float)i / (float)segments, 1, 1), 10, false);
            }

        }
        RaycastHit2D[] rayHitsArray = rayHitsList.ToArray();
        return Physics2DExtensions.SortRaycastHit2D(rayHitsArray);
    }
}
