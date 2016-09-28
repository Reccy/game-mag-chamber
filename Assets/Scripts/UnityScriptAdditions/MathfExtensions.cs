using UnityEngine;
using System.Collections;

namespace UnityExtensions.MathfExtensions
{
    public static class MathfExtensions
    {
        //Credit for Decibel code: http://answers.unity3d.com/questions/283192/how-to-convert-decibel-number-to-audio-source-volu.html

        public static float LinearToDecibel(float linear)
        {
            float dB;

            if (linear != 0)
                dB = 20.0f * Mathf.Log10(linear);
            else
                dB = -144.0f;

            return dB;
        }

        public static float DecibelToLinear(float dB)
        {
            float linear = Mathf.Pow(10.0f, dB / 20.0f);

            return linear;
        }

        public static float AngleFromTo(GameObject obj1, GameObject obj2)
        {
            Vector3 returnAngle = obj1.transform.eulerAngles.x < obj2.transform.position.x - obj1.transform.position.x ? new Vector3(0, 0, 360 - Vector2.Angle((Vector2)obj2.transform.position - (Vector2)obj1.transform.position, Vector2.up)) : new Vector3(0, 0, Vector2.Angle((Vector2)obj2.transform.position - (Vector2)obj1.transform.position, Vector2.up));
            return returnAngle.z;
        }

    }
}
