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

    //State Machine
    enum MenuState {Closed, Options, Quit};
    StateMachine<MenuState> menuState;

    //Init Menu
    void Awake()
    {
        optionsPanelRect = optionsPanel.GetComponent<RectTransform>();
        quitPanelRect = quitPanel.GetComponent<RectTransform>();
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

    void Closed_Enter()
    {
        optionsPanelRect.anchoredPosition = new Vector2(optionsClosedX, optionsPanelRect.anchoredPosition.y);
        quitPanelRect.anchoredPosition = new Vector2(quitClosedX, quitPanelRect.anchoredPosition.y);
    }

    void Options_Enter()
    {
        optionsPanelRect.anchoredPosition = new Vector2(optionsOpenX, optionsPanelRect.anchoredPosition.y);
        quitPanelRect.anchoredPosition = new Vector2(quitClosedX, quitPanelRect.anchoredPosition.y);
    }

    void Quit_Enter()
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
}
