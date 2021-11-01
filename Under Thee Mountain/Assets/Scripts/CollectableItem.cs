using System;
using UnityEngine;

public class CollectableItem : CheckPlayerCollision
{
    public CollectibleType CollectibleObject;
    public enum CollectibleType
    {
        Health,
        Bullet
    }
    
    public PlayerShooting PlayerShooting;
    private PlayerController _playerController;
    private Collider _collider;

    private void Start()
    {
        _playerController = PlayerController.Instance;
        _collider = GetComponent<Collider>();
    }

    private void Update()
    {
        if (!CheckForObject(_collider, Vector3.up, PlayerLayerMask, 0.5f)) return;
        switch (CollectibleObject)
        {
            case CollectibleType.Bullet:
                PlayerShooting.CurrentMagazineCount++;
                break;
            case CollectibleType.Health:
                _playerController.CurrentHealth += 10;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        Destroy(transform.parent.gameObject);
    }
}
