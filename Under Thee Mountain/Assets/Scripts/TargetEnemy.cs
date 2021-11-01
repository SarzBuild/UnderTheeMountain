using UnityEngine;

public class TargetEnemy : MonoBehaviour
{
    public int ScoreGive;
    private PlayerUIManager _playerUiManager;

    private void Start()
    {
        _playerUiManager = PlayerUIManager.Instance;
    }
    
    public void Interacted()
    {
        _playerUiManager.TargetFound++;
        _playerUiManager.ScoreCountNumber += ScoreGive;
        Destroy(transform.parent.gameObject);
    }
}
