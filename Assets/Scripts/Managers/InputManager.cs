using UnityEngine;
using System.Collections;

public class InputManager : MonoBehaviour {

    Vector2 mousePosition;

    void Awake()
    {
        mousePosition = Vector2.zero;
    }

	void Update()
    {
        if(Camera.main != null)
        {
            mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }
    }

    public Vector2 GetMousePosition()
    {
        return mousePosition;
    }
}
