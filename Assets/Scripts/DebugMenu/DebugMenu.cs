using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class DebugMenu : MonoBehaviour {

    GraphicsManager graphics;
    GameManager game;

    public int resWidth, resHeight;
    public bool fullscreen;

    void Awake()
    {
        graphics = Object.FindObjectOfType<GraphicsManager>();
        game = Object.FindObjectOfType<GameManager>();
    }

    public void UpdateResolution()
    {
        Debug.Log("Setting resolution: " + resWidth + ", " + resHeight);
        Screen.SetResolution(resWidth, resHeight, fullscreen);
        Debug.Log("New Resolution: " + resWidth + ", " + resHeight);
    }

    public void SetResWidth(InputField rw)
    {
        resWidth = int.Parse(rw.text);
        Debug.Log("RES WIDTH:" + resWidth);
    }

    public void SetResHeight(InputField rh)
    {
        resHeight = int.Parse(rh.text);
        Debug.Log("RES HEIGHT:" + resHeight);
    }

    public void SetFullscreen(Toggle f)
    {
        fullscreen = f.isOn;
    }
}
