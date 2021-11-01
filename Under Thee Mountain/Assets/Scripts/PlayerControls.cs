using UnityEngine;

public class PlayerControls : MonoBehaviour
{
    [SerializeField] protected bool LockPlayer;
    [SerializeField] protected bool LockMouse;

    protected bool GetMovingUp() => !LockPlayer && (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow));
    protected bool GetMovingRight() => !LockPlayer && (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow));
    protected bool GetMovingLeft() => !LockPlayer && (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow));
    protected bool GetMovingDown() => !LockPlayer && (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow));
    protected bool GetJumpInput() => !LockPlayer && Input.GetKey(KeyCode.Space);
    protected bool GetRightClick() => !LockMouse && Input.GetKeyDown(KeyCode.Mouse1);
    protected bool GetLeftClick() => !LockMouse && Input.GetKeyDown(KeyCode.Mouse0);
    protected bool GetStrafeInput() => !LockPlayer && Input.GetKeyDown(KeyCode.LeftShift);
    protected bool GetEscape() => !LockPlayer && Input.GetKeyDown(KeyCode.Escape);
    protected Vector3 GetMousePos()
    {
        if (LockMouse) return Vector3.zero;
        
        var mousePos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10));
        return mousePos;
    }
    
}
