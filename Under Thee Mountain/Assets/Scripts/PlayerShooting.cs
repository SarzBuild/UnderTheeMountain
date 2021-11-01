using System.Collections;
using UnityEngine;

public class PlayerShooting : PlayerControls
{
    [Header("References")]
    public GameObject AimEndPoint;
    public GameObject Flash;
    public GameObject FakeBulletPrefab;
    public GameObject HitEffect;
    
    private PlayerController _playerController;
    
    [Header("Layer Masks")]
    public LayerMask TargetLayerMask;
    public LayerMask EnemyLayerMask;
    
    [Header("Ammo")]
    public int CurrentAmmoInMagazineCount;
    public int CurrentMagazineCount;
    private const int MaxAmmoInMagazineCount = 8;
    
    [Header("Others")]
    private Vector3 _targetPosition;

    private void Start()
    {
        _playerController = PlayerController.Instance;
    }

    private void Update()
    {
        HandleShooting();
        HandleReloading();
    }
    
    private void HandleShooting()
    {
        if (!GetLeftClick()) return;
        if (CurrentAmmoInMagazineCount <= 0) return;
        
        CurrentAmmoInMagazineCount--;
        StartCoroutine(MuzzleFlash());
        RaycastHit hit;
        Physics.Raycast(
            AimEndPoint.transform.position, 
            _playerController.AimDirection, 
            out hit, 
            Vector3.Distance(
                AimEndPoint.transform.position, 
                new Vector3(
                    GetMousePos().x, 
                    AimEndPoint.transform.position.y, 
                    GetMousePos().z))+0.25f);
        if (hit.collider == null)
        {
            _targetPosition = new Vector3(
                GetMousePos().x, 
                AimEndPoint.transform.position.y,
                GetMousePos().z);
            Debug.DrawLine(AimEndPoint.transform.position, _targetPosition, Color.red, 5f);
        }
        else
        {
            _targetPosition = new Vector3(
                hit.point.x, 
                AimEndPoint.transform.position.y, 
                hit.point.z);
            Debug.DrawLine(AimEndPoint.transform.position, _targetPosition, Color.blue, 5f);
            if ((EnemyLayerMask.value & (1 << hit.collider.gameObject.layer)) > 0)
            {
                var enemyBrain = hit.collider.GetComponent<EnemyBrain>();
                if (enemyBrain != null)
                {
                    enemyBrain.GetDamaged(40);
                    Instantiate(HitEffect, hit.point, new Quaternion(
                        hit.collider.transform.rotation.x,
                        hit.collider.transform.rotation.y-90,
                        hit.collider.transform.rotation.z,
                        hit.collider.transform.rotation.w));
                }
            }
            if ((TargetLayerMask.value & (1 << hit.collider.gameObject.layer)) > 0)
            {
                var targetEnemy = hit.collider.GetComponent<TargetEnemy>();
                if (targetEnemy != null)
                {
                    targetEnemy.Interacted();
                }
            }
        }
    }
    
    private IEnumerator MuzzleFlash()
    {
        Flash.SetActive(true);
        Instantiate(FakeBulletPrefab, AimEndPoint.transform.position, AimEndPoint.transform.rotation);
        yield return new WaitForSeconds(0.2f);
        Flash.SetActive(false);
        StopCoroutine(MuzzleFlash());
    }

    private void HandleReloading()
    {
        if (CurrentAmmoInMagazineCount != 0) return;
        if (!GetRightClick() || CurrentMagazineCount <= 0) return;
        CurrentAmmoInMagazineCount = MaxAmmoInMagazineCount;
        CurrentMagazineCount--;
    }
}
