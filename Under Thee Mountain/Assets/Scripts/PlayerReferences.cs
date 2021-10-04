using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerReferences : MonoBehaviour
{
    private static PlayerReferences _instance;
    public static PlayerReferences Instance {
        get
        {
            if (_instance == null)
            {
                PlayerReferences singleton = GameObject.FindObjectOfType<PlayerReferences>();
                if (singleton == null)
                {
                    GameObject go = new GameObject();
                    _instance = go.AddComponent<PlayerReferences>();
                }
            }
            return _instance;
        }
    }
    
    public int MaxHealth;
    public int CurrentHealth;

    public int TargetFound;
    
    public int MaxAmmoInMagazineCount = 8;
    public int CurrentAmmoInMagazineCount;
    public int MaxMagazineCount = 3;
    public int CurrentMagazineCount;
    public int ScoreCount;
    
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
        
        CurrentHealth = MaxHealth;
    }
    
    
}
