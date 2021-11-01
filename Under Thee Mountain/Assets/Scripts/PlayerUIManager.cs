using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUIManager : PlayerControls
{
    private static PlayerUIManager _instance;
    public static PlayerUIManager Instance {
        get
        {
            if (_instance != null) return _instance;
            
            var singleton = FindObjectOfType<PlayerUIManager>();
            if (singleton != null) return _instance;
            
            var go = new GameObject();
            _instance = go.AddComponent<PlayerUIManager>();
            return _instance;
        }
    }
    
    [Header("Text Related")]
    public TextMeshProUGUI BulletCount;
    public TextMeshProUGUI ChargerCount;
    public TextMeshProUGUI ScoreCount;
    public TextMeshProUGUI HealthCount;
    public TextMeshProUGUI TotalTargets;
    public TextMeshProUGUI TargetsFound;
    
    [Header("References")]
    private PlayerController _playerController;
    public PlayerShooting PlayerShooting;
    public Image HealthImage;
    public List<Transform> TargetEnemies;
    public Image EnemyHealthAmountBackground;
    public Image EnemyHealthAmountSlice;

    private Transform _tempScoreCountTransform;

    [Header("Layer Masks")] 
    public LayerMask EnemyLayerMask;

    [Header("Counters")]
    public int TargetFound;
    public int ScoreCountNumber;

    private int _tempBulletCount;
    private int _tempChargerCount;
    private int _tempScoreCount;
    private int _tempCurrentHealth;
    private int _targetInScene;

    private void Awake()
    {
        if (_instance != null && _instance != this) Destroy(gameObject);
        else _instance = this;
    }
    
    
    private void Start()
    {
        _playerController = PlayerController.Instance;

        _tempBulletCount = PlayerShooting.CurrentAmmoInMagazineCount;
        _tempChargerCount = PlayerShooting.CurrentMagazineCount;
        _tempScoreCount = ScoreCountNumber;
        _tempCurrentHealth = _playerController.CurrentHealth;
        _targetInScene = TargetFound;
        _tempScoreCountTransform = ScoreCount.transform;
        
        TotalTargetsFinder();
    }
    
    private void Update()
    {
        UpdateBullets();
        UpdateChargers();
        UpdateScore();
        UpdateLife();
        UpdateTargets();
        HandleShowHealthOfEnemies();
    }

    private void HandleShowHealthOfEnemies()
    {
        var ray = new Vector3(GetMousePos().x, Camera.main.transform.position.y - 5f,GetMousePos().z);
        Physics.Raycast(ray, Vector3.down, out var hit, Mathf.Infinity, EnemyLayerMask);
        var successfulHit = false;
        if (hit.collider != null)
        {
            var enemyBrain = hit.collider.GetComponent<EnemyBrain>();
            if (enemyBrain != null)
            {
                EnemyHealthAmountBackground.gameObject.SetActive(true);
                EnemyHealthAmountSlice.fillAmount = Mathf.Clamp01(Mathf.InverseLerp(0, enemyBrain.MaxHealth, enemyBrain.CurrentHealth));
                const float offset = 1f;
                if (EnemyHealthAmountBackground.transform.position != new Vector3(hit.point.x ,hit.point.y + offset, hit.point.z))
                {
                    EnemyHealthAmountBackground.transform.position = new Vector3(hit.point.x ,hit.point.y + offset, hit.point.z);
                }
                successfulHit = true;
            }
        }
        if(!successfulHit) EnemyHealthAmountBackground.gameObject.SetActive(false);
    }

    private void TotalTargetsFinder()
    {
        if (TargetEnemies != null && TargetEnemies.Count > 0) TotalTargets.text = TargetEnemies.Count.ToString();
    }

    private void UpdateTargets()
    {
        if (TargetFound == _targetInScene) return;
        
        TargetsFound.text = TargetFound.ToString();
        _targetInScene = TargetFound;
    }

    private void UpdateBullets()
    {
        if (PlayerShooting.CurrentAmmoInMagazineCount == _tempBulletCount) return;
        
        BulletCount.text = string.Concat(Enumerable.Repeat("I", PlayerShooting.CurrentAmmoInMagazineCount));
        _tempBulletCount = PlayerShooting.CurrentAmmoInMagazineCount;
    }

    private void UpdateChargers()
    {
        if (PlayerShooting.CurrentMagazineCount == _tempChargerCount) return;
        
        ChargerCount.text = string.Concat(Enumerable.Repeat("©", PlayerShooting.CurrentMagazineCount));
        _tempChargerCount = PlayerShooting.CurrentMagazineCount;
    }

    private void UpdateScore()
    {
        if (_tempScoreCount >= ScoreCountNumber) return;
        
        StartCoroutine(ScoreJuicer());
        ScoreCount.text = _tempScoreCount.ToString();
    }

    private void UpdateLife()
    {
        if (_tempCurrentHealth <= _playerController.CurrentHealth) return;
        
        HealthCount.text = _playerController.CurrentHealth.ToString("000");
        HealthImage.fillAmount = Mathf.Clamp01(Mathf.InverseLerp(0, _playerController.MaxHealth, _playerController.CurrentHealth));
        _tempCurrentHealth = _playerController.CurrentHealth;
    }

    private IEnumerator ScoreJuicer()
    {
        _tempScoreCount++;
        yield return new WaitForEndOfFrame();
        TweenJuicer();
        StopCoroutine(ScoreJuicer());
    }

    private void TweenJuicer()
    {
        ScoreCount.transform.DORewind();
        ScoreCount.transform.DOPunchScale(Vector3.one, 0.2f).OnComplete(ResetTween);
    }
    private void ResetTween()
    {
        DOTween.Kill(ScoreCount.transform);
    }
}
