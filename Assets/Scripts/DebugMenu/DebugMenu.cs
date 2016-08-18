using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class DebugMenu : MonoBehaviour {

    public int resWidth, resHeight;
    public bool fullscreen;

    //Scene-Load Settings
    public void LoadScene(Text textObj)
    {
        SceneManager.LoadScene(textObj.text);
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
}
