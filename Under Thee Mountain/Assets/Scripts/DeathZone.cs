using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathZone : MonoBehaviour
{
    public LayerMask PlayerLayerMask;
    private PlayerReferences _playerReferences;

    private void Start()
    {
        _playerReferences = PlayerReferences.Instance;
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if ((PlayerLayerMask.value & (1 << other.gameObject.layer)) > 0)
        {
            _playerReferences.CurrentHealth = 0;
        }
    }
}
