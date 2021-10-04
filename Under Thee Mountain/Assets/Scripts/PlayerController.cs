using System.Collections;
using System.Collections.Generic;
using Unity.Jobs;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.TextCore;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    [Header("References")]
    public Collider PlayerCollider;
    public Rigidbody Rigidbody;
    
    private PlayerControls _playerControls;
    private PlayerReferences _playerReferences;
    
    [Header("Physics")]
    public float WalkSpeed = 4f;
    public float JumpingSpeed;
    public float Gravity = -9.81f;
    public float JumpForce = 12f;
    private float _jumpAndFallVelocity;
    
    private Vector3 _aimDirection;
    private Vector3 _rotationNeeded;
    private Vector3 _targetPosition;

    private bool _coroutineRunning;
    private bool _coroutineRunning2;
    
    [Header("Shooting Related")]
    public GameObject AimEndPoint;
    public GameObject Flash;
    
    [Header("Layer Masks")]
    public LayerMask ShootingLayerMask;
    public LayerMask TargetLayerMask;
    public LayerMask GroundLayerMask;
    public LayerMask EnemyLayerMask;
    
    [Header("UI")]
    public Image EnemyHealthAmountBackground;
    public Image EnemyHealthAmountSlice;
    public PauseMenuManager PauseMenuManager;

    private void Awake()
    {
        PlayerCollider = GetComponent<Collider>();
        Rigidbody = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        _playerControls = PlayerControls.Instance;
        _playerReferences = PlayerReferences.Instance;
    }
    
    private void Update()
    {
        HandleRotation();
        HandleShooting();
        HandleReloading();
        HandleShowHealthOfEnemies();
        HandleLife();
    }
    private void FixedUpdate()
    {
        HandleMovement();
        HandleJump();
        HandleFall();
        DecreaseJumpSpeed();
    }
    
    private void HandleMovement()
    {
        Vector3 moveDirection = new Vector3(0, 0);
        if (_playerControls.GetMovingUp())
            moveDirection.z = +1;
        if (_playerControls.GetMovingDown())
            moveDirection.z = -1;
        if (_playerControls.GetMovingLeft())
            moveDirection.x = -1;
        if (_playerControls.GetMovingRight())
            moveDirection.x = +1;
        moveDirection.Normalize();

        Vector3 moveTowardsPosition =
            new Vector3(moveDirection.x * WalkSpeed, _jumpAndFallVelocity, moveDirection.z * WalkSpeed);
        Rigidbody.velocity = moveTowardsPosition;
    }

    private void HandleJump()
    {
        if (_playerControls.GetJumpInput())
        {
            if (CheckIfGrounded())
            {
                if (_jumpAndFallVelocity < 0.1)
                {
                    StartCoroutine(SetJumpSpeedCoroutine());
                    _jumpAndFallVelocity += (WalkSpeed * JumpForce) * 0.2f;
                }
            }
        }
    }
    
    private IEnumerator SetJumpSpeedCoroutine()
    {
        _coroutineRunning = true;
        JumpingSpeed = 1.5f;
        yield return new WaitForSeconds(1f);
        _coroutineRunning = false;
    }
    
    private void DecreaseJumpSpeed()
    {
        if (_coroutineRunning)
            if (JumpingSpeed > 1f)
                JumpingSpeed -= Time.fixedDeltaTime * 0.5f;
            else
                JumpingSpeed = 1f;
    }

    private void HandleFall()
    {
        if (!CheckIfGrounded())
            _jumpAndFallVelocity += (Gravity * WalkSpeed / 2) * Time.deltaTime;
        else if (CheckIfGrounded())
        {
            if (_jumpAndFallVelocity <= 0f)
            {
                ResetJumpVariablesAndCr();
            }
        }
    }

    private void ResetJumpVariablesAndCr()
    {
        _jumpAndFallVelocity = 0f;
        JumpingSpeed = 1f;
        if (_coroutineRunning)
        {
            StopCoroutine(SetJumpSpeedCoroutine());
            _coroutineRunning = false;
        }
    }

    private void HandleRotation()
    {
        Vector3 mousePosition = _playerControls.GetMousePos();
        var angleOffset = 90f;

        _aimDirection = (mousePosition - transform.position).normalized;
        float angle = Mathf.Atan2(_aimDirection.x, _aimDirection.z) * Mathf.Rad2Deg;
        _rotationNeeded = new Vector3(0, angle - angleOffset, 0);
        transform.eulerAngles = _rotationNeeded;
    }

    private void HandleShooting()
    {
        if (_playerControls.GetLeftClick())
        {
            if (_playerReferences.CurrentAmmoInMagazineCount > 0)
            {
                _playerReferences.CurrentAmmoInMagazineCount--;
                StartCoroutine(MuzzleFlash());
                RaycastHit hit;
                Physics.Raycast(
                    AimEndPoint.transform.position,
                    _aimDirection,
                    out hit,
                    Vector3.Distance(
                        AimEndPoint.transform.position,
                        _playerControls.GetMousePos()),
                    ShootingLayerMask);
                if (hit.collider == null)
                {
                    _targetPosition = new Vector3(_playerControls.GetMousePos().x, transform.position.y,
                        _playerControls.GetMousePos().z);
                    Debug.DrawLine(AimEndPoint.transform.position, _targetPosition, Color.red, 5f);
                }
                else
                {
                    _targetPosition = new Vector3(hit.point.x, transform.position.y, hit.point.z);
                    Debug.DrawLine(AimEndPoint.transform.position, _targetPosition, Color.blue, 5f);
                    if ((EnemyLayerMask.value & (1 << hit.collider.gameObject.layer)) > 0)
                    {
                        EnemyBrain enemyBrain = hit.collider.GetComponent<EnemyBrain>();
                        if (enemyBrain != null)
                        {
                            enemyBrain.GetDamaged(40);
                        }
                    }
                    if ((TargetLayerMask.value & (1 << hit.collider.gameObject.layer)) > 0)
                    {
                        Debug.Log("Helloooo");
                        TargetEnemy targetEnemy = hit.collider.GetComponent<TargetEnemy>();
                        if (targetEnemy != null)
                        {
                            Debug.Log("Helloooo222");
                            targetEnemy.Interacted();
                        }
                    }
                }
            }
        }
    }

    private IEnumerator MuzzleFlash()
    {
        Flash.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        Flash.SetActive(false);
        StopCoroutine(MuzzleFlash());
    }

    private void HandleReloading()
    {
        if (_playerReferences.CurrentAmmoInMagazineCount == 0)
        {
            if (_playerControls.GetRightClick() && _playerReferences.CurrentMagazineCount > 0)
            {
                _playerReferences.CurrentAmmoInMagazineCount = _playerReferences.MaxAmmoInMagazineCount;
                _playerReferences.CurrentMagazineCount--;
            }
        }
    }

    private void HandleLife()
    {
        if (_playerReferences.CurrentHealth > _playerReferences.MaxHealth)
        {
            _playerReferences.CurrentHealth = _playerReferences.MaxHealth;
        }
        else if (_playerReferences.CurrentHealth <= 0)
        {
            _playerReferences.CurrentHealth = 0;
            Death();
        }
    }
    
    private void GetDamaged()
    {
        _playerReferences.CurrentHealth -= 10;
    }

    private void Death()
    {
        PauseMenuManager.UIMenuOnDeath();
    }

    private void HandleShowHealthOfEnemies()
    {
        Vector3 ray = new Vector3(_playerControls.GetMousePos().x, _playerControls.MainCamera.transform.position.y - 5f,_playerControls.GetMousePos().z);
        RaycastHit hit;
        Physics.Raycast(ray, Vector3.down, out hit, Mathf.Infinity, EnemyLayerMask);
        var successfulHit = false;
        if (hit.collider != null)
        {
            EnemyBrain enemyBrain = hit.collider.GetComponent<EnemyBrain>();
            if (enemyBrain != null)
            {
                EnemyHealthAmountBackground.gameObject.SetActive(true);
                EnemyHealthAmountSlice.fillAmount = Mathf.Clamp01(Mathf.InverseLerp(0, enemyBrain.MaxHealth, enemyBrain.CurrentHealth));
                var offset = 1f;
                if (EnemyHealthAmountBackground.transform.position != new Vector3(hit.point.x ,hit.point.y + offset, hit.point.z))
                {
                    EnemyHealthAmountBackground.transform.position = new Vector3(hit.point.x ,hit.point.y + offset, hit.point.z);
                }
                successfulHit = true;
            }
        }
        if(!successfulHit)
        {
            EnemyHealthAmountBackground.gameObject.SetActive(false);
        }
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if ((EnemyLayerMask.value & (1 << other.gameObject.layer)) > 0)
        {
            GetDamaged();
        }
    }

    private bool CheckIfGrounded()
    {
        return CollisionCheck(new Vector3(0, -0.5f, 0), 1f, 0.5f, GroundLayerMask);
    }

    private bool CollisionCheck(Vector3 direction,float radius , float maxLength, LayerMask layerMask)
    {
        RaycastHit hit;
        Ray ray = new Ray(transform.position,direction);
        Physics.SphereCast(
            ray,
            radius,
            out hit,
            maxLength,
            layerMask
        ); 
        if(hit.collider != null)
            return true;
        else
            return false;
    }
    
}
