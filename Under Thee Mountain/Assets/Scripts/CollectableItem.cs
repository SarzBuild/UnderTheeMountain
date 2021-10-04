using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CollectableItem : MonoBehaviour
{
    private PlayerReferences _playerReferences;
    public LayerMask PlayerLayerMask;


    public CollectibleType CollectibleObject;
    public enum CollectibleType
    {
        Health,
        Bullet
    }

    private void Start()
    {
        _playerReferences = PlayerReferences.Instance;
    }

    private void OnTriggerEnter(Collider other)
    {
        if ((PlayerLayerMask.value & (1 << other.gameObject.layer)) > 0)
        {
            switch (CollectibleObject)
            {
                case CollectibleType.Bullet:
                    _playerReferences.CurrentMagazineCount++;
                    break;
                case CollectibleType.Health:
                    _playerReferences.CurrentHealth += 10;
                    break;
            }
            Destroy(transform.parent.gameObject);
        }
    }
}
