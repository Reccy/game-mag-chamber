﻿using UnityEngine;
using System.Collections;

public class InputManager : MonoBehaviour {

    GameManager gameManager;
    Vector2 mousePosition;

    void Awake()
    {
        gameManager = Object.FindObjectOfType<GameManager>();
        mousePosition = Vector2.zero;
    }

	void Update()
    {
        //Update the mouse's position from the camera
        if(Camera.main != null && gameManager.gameState.State != GameManager.GameState.Paused)
        {
            mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }

        //Game pause
        if (gameManager.gameState.State == GameManager.GameState.Running && Input.GetKeyDown(KeyCode.Escape))
        {
            gameManager.gameState.ChangeState(GameManager.GameState.Paused);
        }

        //Game resume
        else if (gameManager.gameState.State == GameManager.GameState.Paused && Input.GetKeyDown(KeyCode.Escape))
        {
            gameManager.gameState.ChangeState(GameManager.GameState.Running);
        }
    }

    //Returns the mouse's position on screen
    public Vector2 GetMousePosition()
    {
        return mousePosition;
    }

    //Returns mouse angle's z-axis
    public float GetMouseAngleFrom(GameObject inObj)
    {
        Vector3 returnAngle = inObj.transform.eulerAngles.x < mousePosition.x - inObj.transform.position.x ? new Vector3(0, 0, 360 - Vector2.Angle(mousePosition - (Vector2)inObj.transform.position, Vector2.up)) : new Vector3(0, 0, Vector2.Angle(mousePosition - (Vector2)inObj.transform.position, Vector2.up));
        return returnAngle.z;
    }

    //Returns mouse angle's z-axis as a rad
    public float GetMouseRadFrom(GameObject inObj)
    {
        Vector3 returnAngle = inObj.transform.eulerAngles.x < mousePosition.x - inObj.transform.position.x ? new Vector3(0, 0, 360 - Vector2.Angle(mousePosition - (Vector2)inObj.transform.position, Vector2.up)) : new Vector3(0, 0, Vector2.Angle(mousePosition - (Vector2)inObj.transform.position, Vector2.up));
        return returnAngle.z * Mathf.Deg2Rad;
    }

    //Returns mouse angle's euler rotation
    public Vector3 GetMouseEulerFrom(GameObject inObj)
    {
        return inObj.transform.eulerAngles.x < mousePosition.x - inObj.transform.position.x ? new Vector3(0, 0, 360 - Vector2.Angle(mousePosition - (Vector2)inObj.transform.position, Vector2.up)) : new Vector3(0, 0, Vector2.Angle(mousePosition - (Vector2)inObj.transform.position, Vector2.up));
    }

    //Returns mouse angle's quaternion rotation
    public Quaternion GetMouseQuaternionFrom(GameObject inObj)
    {
        return Quaternion.Euler(GetMouseEulerFrom(inObj));
    }

    //Returns the mouse's distance to the object
    public float GetMouseDistanceFrom(GameObject inObj)
    {
        return Vector2.Distance(inObj.transform.position, mousePosition);
    }

    //Returns the mouse's direction to the object
    public Vector2 GetMouseDirectionFrom(GameObject inObj)
    {
        return mousePosition - (Vector2)inObj.transform.position;
    }

    //Returns mouse sin value
    public float GetMouseSinFrom(GameObject inObj)
    {
        return Mathf.Sin(GetMouseRadFrom(inObj));
    }

    //Returns mouse cos value
    public float GetMouseCosFrom(GameObject inObj)
    {
        return Mathf.Cos(GetMouseRadFrom(inObj));
    }

    //Returns mouse tan value
    public float GetMouseTanFrom(GameObject inObj)
    {
        return Mathf.Tan(GetMouseRadFrom(inObj));
    }

    //Returns mouse SinCosTan value
    public Vector3 GetMouseSinCosTanFrom(GameObject inObj)
    {
        return new Vector3(GetMouseSinFrom(inObj), GetMouseCosFrom(inObj), GetMouseTanFrom(inObj));
    }
}
