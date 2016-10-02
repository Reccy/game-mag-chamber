using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEditor;

[ExecuteInEditMode]
public class ChangeAllUITexts : MonoBehaviour
{
    public Font replacementFont;

    void Awake()
    {
        Text[] textArray = Object.FindObjectsOfType<Text>();

        foreach(Text text in textArray)
        {
            text.font = replacementFont;
        }
    }
}
