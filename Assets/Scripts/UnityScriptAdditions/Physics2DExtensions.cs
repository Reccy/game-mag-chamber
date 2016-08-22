using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace UnityExtensions.Physics2DExtensions
{
    public static class Physics2DExtensions
    {
        public static bool arcCastDebug = false;

        //Sorts Raycast2D array to get closest Raycast2D
        public static RaycastHit2D SortRaycastHit2D(RaycastHit2D[] hits)
        {
            if (hits.Length > 0)
            {
                RaycastHit2D closestHit = hits[0];
                float closestBackDistance = int.MaxValue;
                foreach (RaycastHit2D hit in hits)
                {
                    if (closestBackDistance > hit.distance)
                    {
                        closestHit = hit;
                        closestBackDistance = hit.distance;
                    }
                }
                return closestHit;
            }
            return new RaycastHit2D();
        }

        //Gets closest Raycast2D
        public static RaycastHit2D GetClosestRaycastHit2D(Vector2 origin, Vector2 direction, float distance = Mathf.Infinity, int layerMask = Physics2D.DefaultRaycastLayers, float minDepth = -Mathf.Infinity, float maxDepth = Mathf.Infinity)
        {
            RaycastHit2D[] hits = Physics2D.RaycastAll(origin, direction, distance, layerMask, minDepth, maxDepth);
            return SortRaycastHit2D(hits);
        }

        //Gets raycast around an arc
        public static RaycastHit2D ArcCast(Vector2 origin, int arcStart = 0, int arcRange = 360, int segments = 360, float radius = Mathf.Infinity, int layerMask = Physics2D.DefaultRaycastLayers)
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

                RaycastHit2D rayHit = Physics2D.Raycast(origin, dir, radius, layerMask);

                if (rayHit.transform != null)
                {
                    rayHitsList.Add(rayHit);
                }

                //Debug Code
                if(arcCastDebug)
                {
                    if (i == 0)
                    {
                        Debug.DrawRay(origin, dir * radius, Color.HSVToRGB(0, 1, 1), 10, false);
                    }
                    else
                    {
                        Debug.DrawRay(origin, dir * radius, Color.HSVToRGB((float)i / (float)segments, 1, 1), 10, false);
                    }
                }
            }
            RaycastHit2D[] rayHitsArray = rayHitsList.ToArray();
            return SortRaycastHit2D(rayHitsArray);
        }
    }
}
