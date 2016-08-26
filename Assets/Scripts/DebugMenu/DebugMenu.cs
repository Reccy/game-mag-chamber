using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class DebugMenu : MonoBehaviour {

    public int resWidth, resHeight;
    public bool fullscreen;

    private InputField sceneField;

    GameManager gameManager;

    void Awake()
    {
        gameManager = Object.FindObjectOfType<GameManager>();
        sceneField = GameObject.Find("SceneField").GetComponent<InputField>();
    }

    //Scene-Load Settings
    public void LoadScene(Text textObj)
    {
        gameManager.LoadScene(textObj.text);
    }

    public void SetSceneText(Text textObj)
    {
        sceneField.text = textObj.text;
    }

    //Resolution Settings
    public void UpdateResolution()
    {
        Screen.SetResolution(resWidth, resHeight, fullscreen);
    }

    public void SetResWidth(InputField rw)
    {
        int.TryParse(rw.text, out resWidth);
    }

    public void SetResHeight(InputField rh)
    {
        int.TryParse(rh.text, out resHeight);
    }

    public void SetFullscreen(Toggle f)
    {
        fullscreen = f.isOn;
    }

    //Time Settings
    public void SetTargetFPS(InputField inField)
    {
        int targetFPS;
        int.TryParse(inField.text, out targetFPS);
        Application.targetFrameRate = targetFPS;
    }

    public void SetTimestep(InputField inField)
    {
        int timeStep;
        int.TryParse(inField.text, out timeStep);
        Time.fixedDeltaTime = timeStep;
    }
}
