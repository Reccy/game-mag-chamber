using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class AspectRatioScript_Test : MonoBehaviour
{

    private Camera camera;
    int currentRes;
    string currentResString;

    void Start()
    {
        currentRes = 0;
        camera = GetComponent<Camera>();
        Debug.Log("ASPECT RATIO: " + camera.aspect);
        ApplyAspectRatio();
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }

        if(Input.GetKeyDown(KeyCode.F))
        {
            Screen.fullScreen = !Screen.fullScreen;
        }

        if(Input.GetKeyDown(KeyCode.E))
        {
            currentRes++;

            if(currentRes >= 5)
            {
                currentRes = 0;
            }

            switch(currentRes)
            {
                case 0: //5:4
                    Screen.SetResolution(600, 480, true);
                    currentResString = "600x480";
                    break;
                case 1: //4:3
                    Screen.SetResolution(1024, 768, true);
                    currentResString = "1024x768";
                    break;
                case 2: //3:2
                    Screen.SetResolution(960, 640, true);
                    currentResString = "960x640";
                    break;
                case 3: //16:10
                    Screen.SetResolution(1280, 800, true);
                    currentResString = "1280x800";
                    break;
                case 4: //16:9
                    Screen.SetResolution(1366, 768, true);
                    currentResString = "1366x768";
                    break;
            }

            
        }
        ApplyAspectRatio();
    }

    void ApplyAspectRatio()
    {
        float aspectRatio = camera.aspect;

        if(aspectRatio <= 1.25f) //5:4
        {
            UpdateText(Screen.currentResolution + "\nSMALL");
            camera.orthographicSize = 5.35f;
        }
        else
        {
            UpdateText(Screen.currentResolution + "\nNORMAL");
            camera.orthographicSize = 5f;
        }
            /*
        else if(aspectRatio <= 1.333334f) //4:3
        {
            UpdateText(Screen.currentResolution + "\n4:3");
            camera.orthographicSize = 6.667f;
        }
        else if(aspectRatio <= 1.50f) //3:2
        {
            UpdateText(Screen.currentResolution + "\n3:2");
            camera.orthographicSize = 5.92f;
        }
        else if(aspectRatio <= 1.61f) //16:10
        {
            UpdateText(Screen.currentResolution + "\n16:10");
            camera.orthographicSize = 5.55f;
        }
        else //16:9
        {
            UpdateText(Screen.currentResolution + "\n16:9");
            camera.orthographicSize = 5f;
        }*/
    }

    void UpdateText(string input)
    {
        GameObject.Find("Canvas").GetComponentInChildren<Text>().text = input + "\n" + currentResString;
    }
}