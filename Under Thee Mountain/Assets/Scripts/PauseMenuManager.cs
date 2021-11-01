using TMPro;
using UnityEngine;

public class PauseMenuManager : PlayerControls
{
    public GameObject PauseMenuGameObject;
    public TextMeshProUGUI PauseText;
    public TextMeshProUGUI CauseOfPauseText;
    private bool _endCondition;

    private void Awake()
    {
        Time.timeScale = 1;
    }
    
    private void Update()
    {
        UIMenuOnEscape();
    }

    private void UIMenuOnEscape()
    {
        if (!GetEscape()) return;
        switch (PauseMenuGameObject.activeSelf)
        {
            case true:
                if (!_endCondition) CloseMenu();
                break;
            
            case false:
            {
                if (!_endCondition) ShowMenu("GAME PAUSED","");
                break;
            }
        }
    }

    public void UIMenuOnDeath()
    {
        _endCondition = true;
        ShowMenu("GAME END!", "You died!");
        Time.timeScale = 0;
    }

    public void UIMenuOnWin()
    {
        _endCondition = true;
        ShowMenu("GAME END!", "You won!");
        Time.timeScale = 0;
    }
    
    private void ShowMenu(string pause,string causeOfPause)
    {
        PauseMenuGameObject.SetActive(true);
        
        PauseText.text = pause;
        if (causeOfPause == "") return;
        CauseOfPauseText.gameObject.SetActive(true);
        CauseOfPauseText.text = causeOfPause;
    }

    private void CloseMenu()
    {
        CauseOfPauseText.gameObject.SetActive(false);
        PauseMenuGameObject.SetActive(false);
    }
}
