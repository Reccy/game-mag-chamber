using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using MonsterLove.StateMachine;

public class MainMenu : MonoBehaviour
{
    //Panel management
    public GameObject optionsPanel, optionsButton, quitPanel, quitButton; //UI objects
    RectTransform optionsPanelRect, optionsButtonRect, quitPanelRect, quitButtonRect; //Panel Rects
    public float optionsOpenX, optionsClosedX, optionsButtonOpenX, optionsButtonClosedX, quitOpenX, quitClosedX, quitButtonOpenX, quitButtonClosedX; //Positions for opened and closed panels
    public float animationDuration = 20f; //How long it should take for a tweening animation to complete
    public bool isInitialized = false;

    //Options management
    public Options options;

    //GM classes
    SoundManager sound;

    //State Machine
    enum MenuState { Closed, Options, Quit };
    StateMachine<MenuState> menuState;

    //Init Menu
    void Awake()
    {
        sound = Object.FindObjectOfType<SoundManager>();
        optionsPanelRect = optionsPanel.GetComponent<RectTransform>();
        optionsButtonRect = optionsButton.GetComponent<RectTransform>();
        quitPanelRect = quitPanel.GetComponent<RectTransform>();
        quitButtonRect = quitButton.GetComponent<RectTransform>();
        InitOptionsMenu();
        menuState = StateMachine<MenuState>.Initialize(this);
        menuState.ChangeState(MenuState.Closed);

        //Update UI Time
        if(!PlayerPrefs.HasKey("HighScore"))
            PlayerPrefs.SetFloat("HighScore", 0);

        PlayerPrefs.Save();

        Text timeText = GameObject.Find("BestTimeNumText").GetComponent<Text>();
        Text msText = GameObject.Find("BestTimeMillisecondText").GetComponent<Text>();

        int minutes, seconds;
        float milliseconds;
        string millisecondsString;
        float score = PlayerPrefs.GetFloat("HighScore");
        minutes = (int)Mathf.Floor(score / 60);
        seconds = (int)score - (minutes * 60);
        milliseconds = score - seconds;

        millisecondsString = milliseconds.ToString() + "00000";
        millisecondsString = millisecondsString.Substring(1, 4);
        
        if (milliseconds == 0)
            millisecondsString = ".000";

        msText.text = millisecondsString;

        if (minutes < 10)
        {
            if (seconds < 10)
            {
                timeText.text = "0" + minutes + ":0" + seconds;
            }
            else
            {
                timeText.text = "0" + minutes + ":" + seconds;
            }
        }
        else
        {
            if (seconds < 10)
            {
                timeText.text = minutes + ":0" + seconds;
            }
            else
            {
                timeText.text = minutes + ":" + seconds;
            }
        }
    }

    //Set positions of GUI
    void Start()
    {
        QuickCloseMenus();
    }

    //Fix dropdown unset bug
    void Update()
    {
        EventSystem.current.SetSelectedGameObject(null);
    }

    //
    //State Management
    //

    void Closed_Enter()
    {
        if(menuState.LastState != MenuState.Closed)
            sound.PlayOneShot("sfx_Click", SoundManager.SoundChannel.UI, 0.6f);
    }

    void Closed_FixedUpdate()
    {
        //Velocities
        float v1 = 0;
        float v2 = 0;
        float v3 = 0;
        float v4 = 0;

        if (optionsPanelRect.anchoredPosition.x != optionsOpenX || quitPanelRect.anchoredPosition.x != quitClosedX)
        {
            optionsPanelRect.anchoredPosition = new Vector2(Mathf.SmoothDamp(optionsPanelRect.anchoredPosition.x, optionsClosedX, ref v1, animationDuration), optionsPanelRect.anchoredPosition.y);
            optionsButtonRect.anchoredPosition = new Vector2(Mathf.SmoothDamp(optionsButtonRect.anchoredPosition.x, optionsButtonClosedX, ref v2, animationDuration), optionsButtonRect.anchoredPosition.y);
            quitPanelRect.anchoredPosition = new Vector2(Mathf.SmoothDamp(quitPanelRect.anchoredPosition.x, quitClosedX, ref v3, animationDuration), quitPanelRect.anchoredPosition.y);
            quitButtonRect.anchoredPosition = new Vector2(Mathf.SmoothDamp(quitButtonRect.anchoredPosition.x, quitButtonClosedX, ref v4, animationDuration), quitButtonRect.anchoredPosition.y);
        }
    }

    void Options_Enter()
    {
        sound.PlayOneShot("sfx_Click", SoundManager.SoundChannel.UI);
    }

    void Options_FixedUpdate()
    {
        //Velocities
        float v1 = 0;
        float v2 = 0;
        float v3 = 0;
        float v4 = 0;

        if (optionsPanelRect.anchoredPosition.x != optionsOpenX || quitPanelRect.anchoredPosition.x != quitClosedX)
        {
            optionsPanelRect.anchoredPosition = new Vector2(Mathf.SmoothDamp(optionsPanelRect.anchoredPosition.x, optionsOpenX, ref v1, animationDuration), optionsPanelRect.anchoredPosition.y);
            optionsButtonRect.anchoredPosition = new Vector2(Mathf.SmoothDamp(optionsButtonRect.anchoredPosition.x, optionsButtonOpenX, ref v2, animationDuration), optionsButtonRect.anchoredPosition.y);
            quitPanelRect.anchoredPosition = new Vector2(Mathf.SmoothDamp(quitPanelRect.anchoredPosition.x, quitClosedX, ref v3, animationDuration), quitPanelRect.anchoredPosition.y);
            quitButtonRect.anchoredPosition = new Vector2(Mathf.SmoothDamp(quitButtonRect.anchoredPosition.x, quitButtonClosedX, ref v4, animationDuration), quitButtonRect.anchoredPosition.y);
        }
    }

    void Quit_Enter()
    {
        sound.PlayOneShot("sfx_Click", SoundManager.SoundChannel.UI);
    }

    void Quit_FixedUpdate()
    {
        //Velocities
        float v1 = 0;
        float v2 = 0;
        float v3 = 0;
        float v4 = 0;

        if (optionsPanelRect.anchoredPosition.x != optionsOpenX || quitPanelRect.anchoredPosition.x != quitClosedX)
        {
            optionsPanelRect.anchoredPosition = new Vector2(Mathf.SmoothDamp(optionsPanelRect.anchoredPosition.x, optionsClosedX, ref v1, animationDuration), optionsPanelRect.anchoredPosition.y);
            optionsButtonRect.anchoredPosition = new Vector2(Mathf.SmoothDamp(optionsButtonRect.anchoredPosition.x, optionsButtonClosedX, ref v2, animationDuration), optionsButtonRect.anchoredPosition.y);
            quitPanelRect.anchoredPosition = new Vector2(Mathf.SmoothDamp(quitPanelRect.anchoredPosition.x, quitOpenX, ref v3, animationDuration), quitPanelRect.anchoredPosition.y);
            quitButtonRect.anchoredPosition = new Vector2(Mathf.SmoothDamp(quitButtonRect.anchoredPosition.x, quitButtonOpenX, ref v4, animationDuration), quitButtonRect.anchoredPosition.y);
        }
    }

    //Immediately snaps menus to positions
    void QuickOpenMenus()
    {
        optionsPanelRect.anchoredPosition = new Vector2(optionsOpenX, optionsPanelRect.anchoredPosition.y);
        quitPanelRect.anchoredPosition = new Vector2(quitOpenX, quitPanelRect.anchoredPosition.y);
    }

    void QuickCloseMenus()
    {
        optionsPanelRect.anchoredPosition = new Vector2(optionsClosedX, optionsPanelRect.anchoredPosition.y);
        quitPanelRect.anchoredPosition = new Vector2(quitClosedX, quitPanelRect.anchoredPosition.y);
    }

    //Input Management
    public void ToggleOptionsMenu()
    {
        if(menuState.State == MenuState.Options)
        {
            menuState.ChangeState(MenuState.Closed);
        }
        else
        {
            menuState.ChangeState(MenuState.Options);
        }
    }

    public void ToggleQuitMenu()
    {
        if (menuState.State == MenuState.Quit)
        {
            menuState.ChangeState(MenuState.Closed);
        }
        else
        {
            menuState.ChangeState(MenuState.Quit);
        }
    }

    //
    // Quit Menu
    //

    public void QuitGame()
    {
        Application.Quit();
    }

    //
    // Options Menu
    //

    //Initialize the Options Menu
    void InitOptionsMenu()
    {
        //Check if PlayePrefs exist, else generate keys
        if (!PlayerPrefs.HasKey("Resolution"))
            PlayerPrefs.SetString("Resolution", "640 x 480");

        if (!PlayerPrefs.HasKey("Fullscreen"))
            PlayerPrefs.SetInt("Fullscreen", 0);

        if (!PlayerPrefs.HasKey("AntiAliasing"))
            PlayerPrefs.SetInt("AntiAliasing", 0);

        if (!PlayerPrefs.HasKey("VSync"))
            PlayerPrefs.SetInt("VSync", 0);

        if (!PlayerPrefs.HasKey("FPS"))
            PlayerPrefs.SetInt("FPS", 60);

        PlayerPrefs.Save();

        //Load player preferences
        string selectedResolution = PlayerPrefs.GetString("Resolution");
        int fullscreen = PlayerPrefs.GetInt("Fullscreen");
        int aaValue = PlayerPrefs.GetInt("AntiAliasing");
        int vsValue = PlayerPrefs.GetInt("VSync");
        int targetFPS = PlayerPrefs.GetInt("FPS");

        //Get Resolution
        options.resolutionDropdown.ClearOptions();

        int i = 0, resIndex = 0;
        foreach (Resolution res in Screen.resolutions)
        {
            //Remove refresh rate from input
            string resString = res.ToString();
            string[] splitRes = resString.Split('@');

            if (selectedResolution.Equals(splitRes[0]))
            {
                resIndex = i;
            }

            options.resolutionDropdown.options.Add(new Dropdown.OptionData(splitRes[0]));
            i++;
        }
        options.resolutionDropdown.captionText = options.resolutionDropdown.transform.Find("Label").GetComponent<Text>();
        options.resolutionDropdown.value = resIndex;

        //Init Window Mode
        options.windowDropdown.ClearOptions();
        options.windowDropdown.options.Add(new Dropdown.OptionData("Windowed"));
        options.windowDropdown.options.Add(new Dropdown.OptionData("Fullscreen"));
        options.windowDropdown.captionText = options.windowDropdown.transform.Find("Label").GetComponent<Text>();
        options.windowDropdown.value = fullscreen;

        //Init AA
        options.aaDropdown.ClearOptions();
        options.aaDropdown.options.Add(new Dropdown.OptionData("Disabled"));
        options.aaDropdown.options.Add(new Dropdown.OptionData("2x MSAA"));
        options.aaDropdown.options.Add(new Dropdown.OptionData("4x MSAA"));
        options.aaDropdown.options.Add(new Dropdown.OptionData("8x MSAA"));
        options.aaDropdown.captionText = options.aaDropdown.transform.Find("Label").GetComponent<Text>();
        options.aaDropdown.value = aaValue;

        //Init VSync
        options.vsDropdown.ClearOptions();
        options.vsDropdown.options.Add(new Dropdown.OptionData("VSync Disabled"));
        options.vsDropdown.options.Add(new Dropdown.OptionData("Wait for VBlank"));
        options.vsDropdown.options.Add(new Dropdown.OptionData("Wait for Second VBlank"));
        options.vsDropdown.captionText = options.vsDropdown.transform.Find("Label").GetComponent<Text>();
        options.vsDropdown.value = vsValue;

        //Init FPS Limit
        options.fpsDropdown.ClearOptions();
        options.fpsDropdown.options.Add(new Dropdown.OptionData("60 FPS"));
        options.fpsDropdown.options.Add(new Dropdown.OptionData("30 FPS"));
        options.fpsDropdown.options.Add(new Dropdown.OptionData("No Limit"));
        options.fpsDropdown.captionText = options.fpsDropdown.transform.Find("Label").GetComponent<Text>();
        switch (targetFPS)
        {
            case 60:
                options.fpsDropdown.value = 0;
                break;
            case 30:
                options.fpsDropdown.value = 1;
                break;
            case 0:
                options.fpsDropdown.value = 2;
                break;
        }
        isInitialized = true;
    }
    
    //Apply saved options
    public void ApplyOptions()
    {
        //Get screen settings
        int screenWidth, screenHeight;
        string selectedResolution = options.resolutionDropdown.options[options.resolutionDropdown.value].text;
        string[] splits = selectedResolution.Split('x');
        int.TryParse(splits[0], out screenWidth);
        int.TryParse(splits[1], out screenHeight);
        bool fullscreen = (options.windowDropdown.value == 0) ? false : true;

        Screen.SetResolution(screenWidth, screenHeight, fullscreen);

        //Get Anti-Aliasing settings
        int aaValue = (int)System.Math.Pow(2, options.aaDropdown.value);

        if (aaValue == 1)
            aaValue = 0;

        QualitySettings.antiAliasing = aaValue;

        //Get VSync settings
        int vsValue = options.vsDropdown.value;

        QualitySettings.vSyncCount = vsValue;

        //Get FPS settings
        int fpsLimit;
        if (options.fpsDropdown.value == 0)
        {
            fpsLimit = 60;
        }
        else if (options.fpsDropdown.value == 1)
        {
            fpsLimit = 30;
        }
        else
        {
            fpsLimit = 0;
        }
        Application.targetFrameRate = fpsLimit;

        //Save options to PlayerPrefs
        PlayerPrefs.SetString("Resolution", selectedResolution);
        PlayerPrefs.SetInt("Fullscreen", (fullscreen ? 1 : 0));
        PlayerPrefs.SetInt("AntiAliasing", aaValue);
        PlayerPrefs.SetInt("VSync", vsValue);
        PlayerPrefs.SetInt("FPS", fpsLimit);
        PlayerPrefs.Save();
    }

    //Load Default Options
    public void LoadDefaults()
    {
        options.resolutionDropdown.value = 0;
        options.windowDropdown.value = 0;
        options.aaDropdown.value = 0;
        options.vsDropdown.value = 0;
        options.fpsDropdown.value = 0;
    }

    //
    //Social Media links
    //

    public void GotoTwitter()
    {
        Application.OpenURL("https://twitter.com/TheReccy");
    }

    public void GotoItchio()
    {
        Application.OpenURL("https://reccy.itch.io/");
    }

    public void GotoMusic()
    {
        Application.OpenURL("http://soundimage.org/");
    }
}

//Options Dropdowns
[System.Serializable]
public class Options
{
    public Dropdown resolutionDropdown;
    public Dropdown windowDropdown;
    public Dropdown aaDropdown;
    public Dropdown vsDropdown;
    public Dropdown fpsDropdown;
}