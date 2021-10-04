using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinZone : MonoBehaviour
{
    public LayerMask PlayerLayerMask;
    public PauseMenuManager PauseMenuManager;
    
    private void OnTriggerEnter(Collider other)
    {
        if ((PlayerLayerMask.value & (1 << other.gameObject.layer)) > 0)
        {
            PauseMenuManager.UIMenuOnWin();
        }
    }
}
