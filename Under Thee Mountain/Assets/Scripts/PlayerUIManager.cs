using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class PlayerUIManager : MonoBehaviour
{
    private PlayerControls _playerControls;
    private PlayerReferences _playerReferences;

    private int _tempBulletCount;
    private int _tempChargerCount;
    private int _tempScoreCount;
    private int _tempCurrentHealth;
    private int _targetInScene;
    private Transform _tempScoreCountTransform;

    public TextMeshProUGUI BulletCount;
    public TextMeshProUGUI ChargerCount;
    public TextMeshProUGUI ScoreCount;
    public TextMeshProUGUI HealthCount;

    public TextMeshProUGUI TotalTargets;
    public TextMeshProUGUI TargetsFound;
    
    
    public Image HealthImage;

    public List<Transform> TargetEnemies;
    
    private void Start()
    {
        _playerControls = PlayerControls.Instance;
        _playerReferences = PlayerReferences.Instance;

        _tempBulletCount = _playerReferences.CurrentAmmoInMagazineCount;
        _tempChargerCount = _playerReferences.CurrentMagazineCount;
        _tempScoreCount = _playerReferences.ScoreCount;
        _tempCurrentHealth = _playerReferences.CurrentHealth;
        _targetInScene = _playerReferences.TargetFound;
        
        _tempScoreCountTransform = ScoreCount.transform;
        
        TotalTargetsFinder();
    }

    private void TotalTargetsFinder()
    {
        if (TargetEnemies != null && TargetEnemies.Count > 0)
        {
            TotalTargets.text = TargetEnemies.Count.ToString();
        }
    }

    private void Update()
    {
        UpdateBullets();
        UpdateChargers();
        UpdateScore();
        UpdateLife();
        UpdateTargets();
    }

    private void UpdateTargets()
    {
        if (_playerReferences.TargetFound != _targetInScene)
        {
            TargetsFound.text = _playerReferences.TargetFound.ToString();
            _targetInScene = _playerReferences.TargetFound;
        }
    }

    private void UpdateBullets()
    {
        if (_playerReferences.CurrentAmmoInMagazineCount != _tempBulletCount)
        {
            BulletCount.text = string.Concat(Enumerable.Repeat("I", _playerReferences.CurrentAmmoInMagazineCount));
            _tempBulletCount = _playerReferences.CurrentAmmoInMagazineCount;
        }
    }

    private void UpdateChargers()
    {
        if (_playerReferences.CurrentMagazineCount != _tempChargerCount)
        {
            ChargerCount.text = string.Concat(Enumerable.Repeat("©", _playerReferences.CurrentMagazineCount));
            _tempChargerCount = _playerReferences.CurrentMagazineCount;
        }
    }

    private void UpdateScore()
    {
        if (_tempScoreCount < _playerReferences.ScoreCount)
        {
            StartCoroutine(ScoreJuicer());
            ScoreCount.text = _tempScoreCount.ToString();
        }
    }

    private void UpdateLife()
    {
        if (_tempCurrentHealth > _playerReferences.CurrentHealth)
        {
            HealthCount.text = _playerReferences.CurrentHealth.ToString("000");
            HealthImage.fillAmount = Mathf.Clamp01(Mathf.InverseLerp(0, _playerReferences.MaxHealth, _playerReferences.CurrentHealth));
            _tempCurrentHealth = _playerReferences.CurrentHealth;
        }
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
        DOTween.Kill(ScoreCount);
    }
}
