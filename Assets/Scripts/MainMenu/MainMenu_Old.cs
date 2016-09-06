using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using MonsterLove.StateMachine;

public class MainMenu_Old : MonoBehaviour {

    //State Machine
    enum MenuState {MainMenu, LevelSelect, Options, Credits, Quit};
    StateMachine<MenuState> menuState;

    //Game Manager
    GameManager gameManager;

    //Panels
    public GameObject mainMenuPanel, levelSelectPanel, optionsPanel, creditsPanel, quitPanel;

    //Levels
    public LevelData[] levels;
    int selectedLevelIndex = 0;

    //Options
    public Options options;

    void Awake()
    {
        //Get Game Manager reference
        gameManager = Object.FindObjectOfType<GameManager>();

        //Initialized Options Menu
        InitOptionsMenu();

        //Gets level data
        UpdateLevelData(selectedLevelIndex);

        //Disable panels
        levelSelectPanel.SetActive(false);
        optionsPanel.SetActive(false);

        //Init FSM
        menuState = StateMachine<MenuState>.Initialize(this);
        menuState.ChangeState(MenuState.MainMenu);
    }

    /*
     * State management
     */

    //Level Select
    void LevelSelect_Enter()
    {
        levelSelectPanel.SetActive(true);
    }

    void LevelSelect_Exit()
    {
        levelSelectPanel.SetActive(false);
    }

    //Options Panel
    void Options_Enter()
    {
        optionsPanel.SetActive(true);
    }

    void Options_Exit()
    {
        optionsPanel.SetActive(false);
    }

    //Credits Panel
    void Credits_Enter()
    {
        creditsPanel.SetActive(true);
    }

    void Credits_Exit()
    {
        creditsPanel.SetActive(false);
    }

    //Quit Panel
    void Quit_Enter()
    {
        quitPanel.SetActive(true);
    }

    void Quit_Exit()
    {
        quitPanel.SetActive(false);
    }

    /*
     * Button management
     */

    //Main Menu
    public void PlayGame()
    {
        menuState.ChangeState(MenuState.LevelSelect);
    }

    public void Options()
    {
        menuState.ChangeState(MenuState.Options);
    }

    public void Credits()
    {
        menuState.ChangeState(MenuState.Credits);
    }

	public void Quit()
    {
        menuState.ChangeState(MenuState.Quit);
    }

    //Level Select
    public void BackToMenu()
    {
        menuState.ChangeState(MenuState.MainMenu);
    }

    public void NextLevel()
    {
        if(selectedLevelIndex == 4)
        {
            selectedLevelIndex = 0;
        }
        else
        {
            selectedLevelIndex++;
        }
        UpdateLevelData(selectedLevelIndex);
    }

    public void PrevLevel()
    {
        if(selectedLevelIndex == 0)
        {
            selectedLevelIndex = 4;
        }
        else
        {
            selectedLevelIndex--;
        }
        UpdateLevelData(selectedLevelIndex);
    }

    //Options
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
        if(options.fpsDropdown.value == 0)
        {
            fpsLimit = 60;
        }
        else if(options.fpsDropdown.value == 1)
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

    //Quit
    public void QuitGame()
    {
        Application.Quit();
    }

    /*
     * Level Select Methods
     */

    void UpdateLevelData(int index)
    {
        levelSelectPanel.transform.Find("LevelNumber").GetComponent<Text>().text = "Level " + (index + 1);
        levelSelectPanel.transform.Find("LevelName").GetComponent<Text>().text = levels[index].levelName;
        levelSelectPanel.transform.Find("LevelImage").GetComponent<Image>().sprite = levels[index].levelImage;
        levelSelectPanel.transform.Find("HighScore").GetComponent<Text>().text = "High Score:\n" + levels[index].highScore;
        levelSelectPanel.transform.Find("DevScore").GetComponent<Text>().text = "Dev Score:\n" + levels[index].devScore;
        levelSelectPanel.transform.Find("StartButton").GetComponent<Button>().interactable = levels[index].isUnlocked;
    }

    public void LoadLevel()
    {
        switch(selectedLevelIndex)
        {
            case 0:
                gameManager.LoadScene("Level1");
                break;
            case 1:
                gameManager.LoadScene("Level2");
                break;
            case 2:
                gameManager.LoadScene("Level3");
                break;
            case 3:
                gameManager.LoadScene("Level4");
                break;
            case 4:
                gameManager.LoadScene("Level5");
                break;
        }
    }

    /*
     * Options Menu Methods
     */

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
        foreach(Resolution res in Screen.resolutions)
        {
            //Remove refresh rate from input
            string resString = res.ToString();
            string[] splitRes = resString.Split('@');

            if(selectedResolution.Equals(splitRes[0]))
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
        switch(targetFPS)
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

//Level Data
[System.Serializable]
public class LevelData
{
    public string levelName;
    public Sprite levelImage;
    public int highScore;
    public int devScore;
    public bool isUnlocked;
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