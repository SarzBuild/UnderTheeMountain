using System.Collections;
using UnityEngine;

public class PlayerController : PlayerControls
{
    private static PlayerController _instance;
    public static PlayerController Instance {
        get
        {
            if (_instance != null) return _instance;
            
            var singleton = FindObjectOfType<PlayerController>();
            if (singleton != null) return _instance;
            
            var go = new GameObject();
            _instance = go.AddComponent<PlayerController>();
            return _instance;
        }
    }
    
    [Header("References")] 
    private Rigidbody _rigidbody;

    [Header("Physics")]
    public float WalkSpeed = 4f;
    public float JumpingSpeed;
    public float DashSpeed;
    public float Gravity = -9.81f;
    public float JumpForce = 12f;
    public Vector3 AimDirection;
    
    private float _jumpAndFallVelocity;
    private bool _resetJumpCoroutine;
    private bool _dashCoroutine;
    private Vector3 _rotationNeeded;
    private Vector3 _tempDashDirection;

    [Header("Layer Masks")]
    public LayerMask GroundLayerMask;
    public LayerMask EnemyLayerMask;
    
    [Header("UI")]
    public PauseMenuManager PauseMenuManager;
    
    [Header("Health")]
    public int MaxHealth;
    public int CurrentHealth;

    private void Awake()
    {
        LockPlayer = false;
        if (_instance != null && _instance != this) Destroy(gameObject);
        else _instance = this;

        CurrentHealth = MaxHealth;
        _rigidbody = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        HandleRotation();
        HandleLife();
        HandleDash();
    }
    private void FixedUpdate()
    {
        HandleMovement();
        HandleJump();
        HandleFall();
        DecreaseJumpSpeed();
        DashMovement();
    }

    private void HandleMovement()
    {
        var moveDirection = new Vector3(0, 0);
        if (GetMovingUp()) moveDirection.z = +1;
        if (GetMovingDown()) moveDirection.z = -1;
        if (GetMovingLeft()) moveDirection.x = -1;
        if (GetMovingRight()) moveDirection.x = +1;
        moveDirection.Normalize();

        var moveTowardsPosition =
            new Vector3(moveDirection.x * WalkSpeed, _jumpAndFallVelocity, moveDirection.z * WalkSpeed);
        _rigidbody.velocity = moveTowardsPosition;
    }

    private void HandleJump()
    {
        if (!GetJumpInput()) return;
        if (!CheckIfGrounded()) return;
        if (!(_jumpAndFallVelocity < 0.1)) return;
        
        StartCoroutine(SetJumpSpeedCoroutine());
        _jumpAndFallVelocity += (WalkSpeed * JumpForce) * 0.2f;
    }
    
    private IEnumerator SetJumpSpeedCoroutine()
    {
        _resetJumpCoroutine = true;
        JumpingSpeed = 1.5f;
        yield return new WaitForSeconds(1f);
        _resetJumpCoroutine = false;
    }
    
    private void DecreaseJumpSpeed()
    {
        if (!_resetJumpCoroutine) return;
        if (JumpingSpeed > 1f) JumpingSpeed -= Time.fixedDeltaTime * 0.5f;
        else JumpingSpeed = 1f;
    }

    private void HandleFall()
    {
        if (!CheckIfGrounded()) _jumpAndFallVelocity += (Gravity * WalkSpeed / 2) * Time.deltaTime;
        else if (CheckIfGrounded() && _jumpAndFallVelocity <= 0f) ResetJumpVariablesAndCoroutine();
    }

    private void ResetJumpVariablesAndCoroutine()
    {
        _jumpAndFallVelocity = 0f;
        JumpingSpeed = 1f;
        if (!_resetJumpCoroutine) return;
        StopCoroutine(SetJumpSpeedCoroutine());
        _resetJumpCoroutine = false;
    }

    private void HandleRotation()
    {
        const float angleOffset = 90f;

        AimDirection = (GetMousePos() - transform.position).normalized;
        var angle = Mathf.Atan2(AimDirection.x, AimDirection.z) * Mathf.Rad2Deg;
        _rotationNeeded = new Vector3(0, angle - angleOffset, 0);
        transform.eulerAngles = _rotationNeeded;
    }

    private void HandleLife()
    {
        if (CurrentHealth > MaxHealth) CurrentHealth = MaxHealth;
        else if (CurrentHealth <= 0)
        {
            CurrentHealth = 0;
            LockPlayer = true;
            LockMouse = true;
            Death();
        }
    }

    private void HandleDash()
    {
        if (!GetStrafeInput()) return;
        if (!CheckIfGrounded()) return;
        if (_dashCoroutine) return;
        _tempDashDirection = new Vector3(_rigidbody.velocity.x * DashSpeed, _rigidbody.velocity.y,_rigidbody.velocity.z * DashSpeed);
        StartCoroutine(Dash());
    }

    private void DashMovement()
    {
        if(!_dashCoroutine) return;
        _rigidbody.velocity = _tempDashDirection;
    }

    private IEnumerator Dash()
    {
        _dashCoroutine = true;
        LockPlayer = true;
        yield return new WaitForSeconds(0.5f);
        LockPlayer = false;
        _dashCoroutine = false;
    }
    
    private void GetDamaged() => CurrentHealth -= 10;

    private void Death() => PauseMenuManager.UIMenuOnDeath();

    private void OnTriggerEnter(Collider other)
    {
        if ((EnemyLayerMask.value & (1 << other.gameObject.layer)) > 0) GetDamaged();
    }

    private bool CheckIfGrounded() => CollisionCheck(new Vector3(0, -0.5f, 0), 1f, 0.5f, GroundLayerMask);
    private bool CollisionCheck(Vector3 direction,float radius , float maxLength, LayerMask layerMask)
    {
        RaycastHit hit;
        var ray = new Ray(transform.position,direction);
        Physics.SphereCast(
            ray,
            radius,
            out hit,
            maxLength,
            layerMask
        ); 
        return hit.collider != null;
    }
    
}
