using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyBrain : MonoBehaviour
{
    [Header("Inventory")] public int MaxHealth = 100;
    public int CurrentHealth;
    public int ScoreToGive;

    [Header("References")] public Transform TargetPlayer;
    public NavMeshAgent NavMeshAgent;
    public Collider EnemyCollider;
    public Transform FleeToTarget;
    public Animator AnimatorComponent;
    public GameObject Ghost;
    public GameObject BloodBath;

    private PlayerUIManager _playerUiManager;

    [Header("Brain Functionalities & Navigation")]
    public float AggroRange;
    public int RunThreshold;
    public float SeeingRange;

    private Vector3 _movementVector;
    private Vector3 _originalPosition;
    private Vector3 _aimDirection;
    private Vector3 _rotationNeeded;
    private Transform _nearestTarget;

    private bool _attackPlayer;
    private bool _possibleGangUp;
    private bool _canMove;

    private bool _deathSequence;

    [Header("Layer Masks")] public LayerMask PlayerLayerMask;
    public LayerMask FriendLayerMask;

    [Header("Detection Lists")] public Collider[] Colliders;
    public List<Transform> NearestTransforms = new List<Transform>();

    private void Awake()
    {
        EnemyCollider = GetComponent<Collider>();
        AnimatorComponent = GetComponentInParent<Animator>();
        CurrentHealth = MaxHealth;
        _canMove = true;
    }

    private void Start()
    {
        _playerUiManager = PlayerUIManager.Instance;
    }

    private void Update()
    {
        HandleLookAtChooseTarget();
        HandleDeath();
        HandleMovement();
        HandleFlee();
        HandleAttack();
        HandleAnimations();
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, SeeingRange);
    }

    private bool HandleSeeingDistancePlayer() => HandleNearDetectionPlayer(SeeingRange);
    private bool HandleSeeingDistanceFriend() => HandleNearDetectionFriends(SeeingRange);
    private bool HandleAggroDistancePlayer() => HandleNearDetectionPlayer(AggroRange);

    private bool HandleNearDetectionPlayer(float radius)
    {
        Colliders = Physics.OverlapSphere(transform.position, radius, PlayerLayerMask);
        return Colliders != null && Colliders.Length > 0;
    }

    private bool HandleNearDetectionFriends(float radius)
    {
        Colliders = Physics.OverlapSphere(transform.position, radius, FriendLayerMask);
        if (Colliders == null || Colliders.Length <= 0) return false;
        foreach (var vcollider in Colliders)
        {
            if (vcollider == EnemyCollider) continue;
            if (!NearestTransforms.Contains(vcollider.transform))
                NearestTransforms.Add(vcollider.transform);
        }
        return true;
    }

    private void HandleLookAtChooseTarget()
    {
        if (HandleSeeingDistancePlayer()) HandleLookAt(TargetPlayer);
        else if (HandleSeeingDistanceFriend()) HandleLookAt(HandleFindNearestFriend());
    }

    private Transform HandleFindNearestFriend()
    {
        _nearestTarget = null;
        if (NearestTransforms == null || NearestTransforms.Count <= 0) return transform;
        
        var closestDistanceSqr = Mathf.Infinity;
        foreach (var possibleTarget in NearestTransforms)
        {
            var directionToTarget = possibleTarget.position - transform.position;
            var dSqrToTarget = directionToTarget.sqrMagnitude;
            if (!(dSqrToTarget < closestDistanceSqr)) continue;
            closestDistanceSqr = dSqrToTarget;
            _nearestTarget = possibleTarget;
        }
        return _nearestTarget;
    }

    private void HandleLookAt(Transform target)
    {
        const float angleOffset = 90f;

        _aimDirection = (target.position - transform.position).normalized;
        var angle = Mathf.Atan2(_aimDirection.x, _aimDirection.z) * Mathf.Rad2Deg;
        _rotationNeeded = new Vector3(0, angle - angleOffset, 0);
        transform.eulerAngles = _rotationNeeded;

    }

    private void HandleMovement()
    {
        if (CurrentHealth > RunThreshold || _possibleGangUp)
        {
            if (!HandleAggroDistancePlayer()) return;
            
            if (NearestTransforms != null && NearestTransforms.Count > 0)
                foreach (var friend in NearestTransforms)
                    friend.GetComponent<EnemyBrain>()._attackPlayer = true;
            _attackPlayer = true;
        }
        else if (HandleSeeingDistancePlayer())
        {
            _attackPlayer = false;
        }
    }

    private void HandleAttack()
    {
        if (_attackPlayer)
        {
            NavMeshAgent.SetDestination(TargetPlayer.position);
        }

    }

    private void HandleFlee()
    {
        if (NearestTransforms.Count > 2) return;
        if (CurrentHealth >= RunThreshold) return;
        
        _attackPlayer = false;
        if (_canMove)
        {
            NavMeshAgent.SetDestination(FleeToTarget.position);
        }
    }

    private void HandleAnimations()
    {
        if (AnimatorComponent == null) return;
        AnimatorComponent.SetBool("Attacking", Vector3.Distance(transform.position, TargetPlayer.position) <= 10f);
        AnimatorComponent.SetBool("Dead", CurrentHealth <= 0);
    }

    private void OnDisable()
    {
        OnDisableUnsubToFriendsLists();
    }

    private void OnDisableUnsubToFriendsLists()
    {
        foreach (var varTransform in NearestTransforms)
        {
            varTransform.GetComponent<EnemyBrain>().NearestTransforms.Remove(transform);
        }
    }

    public void GetDamaged(int damageAmount)
    {
        CurrentHealth -= damageAmount;
    }

    private void HandleDeath()
    {
        if (CurrentHealth > 0) return;
        if (!_deathSequence)
        {
            StartCoroutine(DeathSequence());
        }
    }

    private IEnumerator DeathSequence()
    {
        Instantiate(Ghost, transform.position, transform.rotation);
        Instantiate(BloodBath, transform.position, transform.rotation);
        _deathSequence = true;
        EnemyCollider.enabled = false;
        _canMove = false;
        NavMeshAgent.isStopped = true;
        NavMeshAgent.enabled = false;
        _playerUiManager.ScoreCountNumber += ScoreToGive;
        yield return new WaitForSeconds(1.1f);
        gameObject.SetActive(false);
    }

}
