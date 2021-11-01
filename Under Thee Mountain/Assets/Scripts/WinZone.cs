using UnityEngine;

public class WinZone : CheckPlayerCollision
{
    public PauseMenuManager PauseMenuManager;
    private Collider _collider;

    private void Awake()
    {
        _collider = GetComponent<Collider>();
    }

    private void Update()
    {
        if (CheckForObject(_collider, Vector3.up, PlayerLayerMask, 0.5f))
        {
            PauseMenuManager.UIMenuOnWin();
        }
    }
}
