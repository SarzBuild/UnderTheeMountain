using UnityEngine;

public class DeathZone : CheckPlayerCollision
{
    private PlayerController _playerController;
    private Collider _collider;

    private void Awake()
    {
        _collider = GetComponent<Collider>();
    }
    
    private void Start()
    {
        _playerController = PlayerController.Instance;
    }

    private void Update()
    {
        if (CheckForObject(_collider, Vector3.up, PlayerLayerMask, 0.5f)) _playerController.CurrentHealth = 0;
        
    }
}
