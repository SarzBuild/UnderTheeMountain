using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CollectableItem : MonoBehaviour
{
    private PlayerReferences _playerReferences;
    public LayerMask PlayerLayerMask;

    private void Start()
    {
        _playerReferences = PlayerReferences.Instance;
    }

    private void OnTriggerEnter(Collider other)
    {
        if ((PlayerLayerMask.value & (1 << other.gameObject.layer)) > 0)
        {
            _playerReferences.CurrentMagazineCount++;
            Destroy(transform.parent);
        }
    }
}
