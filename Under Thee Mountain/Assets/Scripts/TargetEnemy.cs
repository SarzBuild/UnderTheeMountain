using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetEnemy : MonoBehaviour
{
    private PlayerReferences _playerReferences;
    public int ScoreGive;
    
    private void Start()
    {
        _playerReferences = PlayerReferences.Instance;
    }
    
    public void Interacted()
    {
        _playerReferences.TargetFound++;
        _playerReferences.ScoreCount += ScoreGive;
        Destroy(transform.parent.gameObject);
    }
}
