using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControls : MonoBehaviour
{
    private bool _lockPlayer;
    public bool CursorVisibility;
    public Camera MainCamera;
    
    
    private static PlayerControls _instance;
    public static PlayerControls Instance {
        get
        {
            if (_instance == null)
            {
                PlayerControls singleton = GameObject.FindObjectOfType<PlayerControls>();
                if (singleton == null)
                {
                    GameObject go = new GameObject();
                    _instance = go.AddComponent<PlayerControls>();
                }
            }
            return _instance;
        }
    } 

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
        Time.timeScale = 1;
    }

    public bool GetMovingUp()
    {
        if (!_lockPlayer)
        {
            if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
            {
                return true;
            }
        }
        return false;
    }
    public bool GetMovingRight()
    {
        if (!_lockPlayer)
        {
            if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
            {
                return true;
            }
        }
        return false;
    }
    public bool GetMovingLeft()
    {
        if (!_lockPlayer)
        {
            if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
            {
                return true;
            }
        }
        return false;
    }
    public bool GetMovingDown()
    {
        if (!_lockPlayer)
        {
            if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
            {
                return true;
            }
        }
        return false;
    }

    public bool GetJumpInput()
    {
        if (!_lockPlayer)
        {
            if (Input.GetKey(KeyCode.Space))
            {
                return true;
            }
        }
        return false;
    }

    public Vector3 GetMousePos()
    {
        if (!_lockPlayer)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                Vector3 vec = hit.point;
                vec.y = 0f;
                return vec;
                
            }
        }
        var nullableVector3 = Vector3.zero;
        return nullableVector3;
    }

    public bool GetRightClick()
    {
        if (!_lockPlayer)
        {
            if (Input.GetKeyDown(KeyCode.Mouse1))
            {
                return true;
            }
        }
        return false;
    }

    public bool GetLeftClick()
    {
        if (!_lockPlayer)
        {
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                return true;
            }
        }
        return false;
    }
    
    public bool GetEscape()
    {
        if (!_lockPlayer)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                return true;
            }
        }
        return false;
    }
}
