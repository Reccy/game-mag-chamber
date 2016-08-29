using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using MonsterLove.StateMachine;

public class MainMenu : MonoBehaviour {

    //State Machine
    enum MenuState {MainMenu, LevelSelect, Options};
    StateMachine<MenuState> menuState;

    //Game Manager
    GameManager gameManager;

    //Panels
    public GameObject mainMenuPanel, levelSelectPanel, optionsPanel;

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

	public void QuitGame()
    {
        Application.Quit();
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
        Debug.Log("Not implemented yet...");
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
        //Init Resolutions
        options.resolutionDropdown.ClearOptions();
        foreach(Resolution res in Screen.resolutions)
        {
            options.resolutionDropdown.options.Add(new Dropdown.OptionData(res.ToString()));
        }
        options.resolutionDropdown.captionText = options.resolutionDropdown.transform.Find("Label").GetComponent<Text>();

        //Init Window Mode
        options.windowDropdown.ClearOptions();
        options.windowDropdown.options.Add(new Dropdown.OptionData("Fullscreen"));
        options.windowDropdown.options.Add(new Dropdown.OptionData("Fullscreen/Windowed"));
        options.windowDropdown.options.Add(new Dropdown.OptionData("Windowed"));
        options.windowDropdown.captionText = options.windowDropdown.transform.Find("Label").GetComponent<Text>();

        //Init AA
        options.aaDropdown.ClearOptions();
        options.aaDropdown.options.Add(new Dropdown.OptionData("Disabled"));
        options.aaDropdown.options.Add(new Dropdown.OptionData("2x MSAA"));
        options.aaDropdown.options.Add(new Dropdown.OptionData("4x MSAA"));
        options.aaDropdown.options.Add(new Dropdown.OptionData("8x MSAA"));
        options.aaDropdown.captionText = options.aaDropdown.transform.Find("Label").GetComponent<Text>();

        //Init VSync
        options.vsDropdown.ClearOptions();
        options.vsDropdown.options.Add(new Dropdown.OptionData("VSync OFF"));
        options.vsDropdown.options.Add(new Dropdown.OptionData("Wait for VBlank"));
        options.vsDropdown.options.Add(new Dropdown.OptionData("Wait for Second VBlank"));
        options.vsDropdown.captionText = options.vsDropdown.transform.Find("Label").GetComponent<Text>();

        //Init FPS Limit
        options.fpsDropdown.ClearOptions();
        options.fpsDropdown.options.Add(new Dropdown.OptionData("60 fps"));
        options.fpsDropdown.options.Add(new Dropdown.OptionData("30 fps"));
        options.fpsDropdown.options.Add(new Dropdown.OptionData("No Limit"));
        options.fpsDropdown.captionText = options.fpsDropdown.transform.Find("Label").GetComponent<Text>();
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

[System.Serializable]
public class Options
{
    public Dropdown resolutionDropdown;
    public Dropdown windowDropdown;
    public Dropdown aaDropdown;
    public Dropdown vsDropdown;
    public Dropdown fpsDropdown;

    public Resolution currentResolution;
    public string windowMode;
    public string aaMode;
    public string vsMode;
    public string fpsLimit;
}