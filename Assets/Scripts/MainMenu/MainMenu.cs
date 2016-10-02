using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using MonsterLove.StateMachine;

public class MainMenu : MonoBehaviour
{
    //Panel management
    public GameObject optionsPanel, quitPanel; //Panels
    RectTransform optionsPanelRect, quitPanelRect; //Panel Rects
    public float optionsOpenX, optionsClosedX, quitOpenX, quitClosedX; //Positions for opened and closed panels

    //Options management
    public Options options;

    //Font
    public Font uiFont;

    //State Machine
    enum MenuState {Closed, Options, Quit};
    StateMachine<MenuState> menuState;

    //Init Menu
    void Awake()
    {
        optionsPanelRect = optionsPanel.GetComponent<RectTransform>();
        quitPanelRect = quitPanel.GetComponent<RectTransform>();
        InitOptionsMenu();
        menuState = StateMachine<MenuState>.Initialize(this);
        menuState.ChangeState(MenuState.Closed);
    }

    //Set positions of GUI
    void Start()
    {
        QuickCloseMenus();
    }

    //
    //State Management
    //

    void Closed_Update()
    {
        optionsPanelRect.anchoredPosition = new Vector2(optionsClosedX, optionsPanelRect.anchoredPosition.y);
        quitPanelRect.anchoredPosition = new Vector2(quitClosedX, quitPanelRect.anchoredPosition.y);
    }

    void Options_Update()
    {
        optionsPanelRect.anchoredPosition = new Vector2(optionsOpenX, optionsPanelRect.anchoredPosition.y);
        quitPanelRect.anchoredPosition = new Vector2(quitClosedX, quitPanelRect.anchoredPosition.y);
    }

    void Quit_Update()
    {
        optionsPanelRect.anchoredPosition = new Vector2(optionsClosedX, optionsPanelRect.anchoredPosition.y);
        quitPanelRect.anchoredPosition = new Vector2(quitOpenX, quitPanelRect.anchoredPosition.y);
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

    //Close menus normally

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