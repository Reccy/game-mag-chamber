using UnityEngine;
using UnityEngine.UI;
using System.Collections;
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

    void Awake()
    {
        //Get Game Manager reference
        gameManager = Object.FindObjectOfType<GameManager>();

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