using System.Collections;
using System.Linq;
using TMPro;
using UnityEngine;
using DG.Tweening;

public class PlayerUIManager : MonoBehaviour
{
    private PlayerControls _playerControls;
    private PlayerReferences _playerReferences;

    private int _tempBulletCount;
    private int _tempChargerCount;
    private int _tempScoreCount;
    private Transform _tempScoreCountTransform;

    public TextMeshProUGUI BulletCount;
    public TextMeshProUGUI ChargerCount;
    public TextMeshProUGUI ScoreCount;
    
    private void Start()
    {
        _playerControls = PlayerControls.Instance;
        _playerReferences = PlayerReferences.Instance;

        _tempBulletCount = _playerReferences.CurrentAmmoInMagazineCount;
        _tempChargerCount = _playerReferences.CurrentMagazineCount;
        _tempScoreCount = _playerReferences.ScoreCount;

        _tempScoreCountTransform = ScoreCount.transform;

    }

    private void Update()
    {
        UpdateBullets();
        UpdateChargers();
        UpdateScore();
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

    private IEnumerator ScoreJuicer()
    {
        _tempScoreCount++;
        yield return new WaitForEndOfFrame();
        TweenJuicer();
        Debug.Log("Helloooo");
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
