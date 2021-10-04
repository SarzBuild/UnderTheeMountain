using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PauseMenuManager : MonoBehaviour
{
    private PlayerControls _playerControls;

    private bool _endCondition;
    
    public GameObject PauseMenuGameObject;
    
    public TextMeshProUGUI PauseText;
    public TextMeshProUGUI CauseOfPauseText;
    

    private void Start()
    {
        _playerControls = PlayerControls.Instance;    
    }

    private void Update()
    {
        UIMenuOnEscape();
    }

    private void UIMenuOnEscape()
    {
        if (_playerControls.GetEscape())
        {
            if (PauseMenuGameObject.activeSelf)
            {
                if (!_endCondition)
                {
                    CloseMenu();
                }
            }
            else if (PauseMenuGameObject.activeSelf == false)
            {
                if (!_endCondition)
                {
                    ShowMenu("GAME PAUSED","");
                }
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
        if (causeOfPause != "")
        {
            CauseOfPauseText.gameObject.SetActive(true);
            CauseOfPauseText.text = causeOfPause;  
        }
    }

    private void CloseMenu()
    {
        CauseOfPauseText.gameObject.SetActive(false);
        PauseMenuGameObject.SetActive(false);
    }
}
