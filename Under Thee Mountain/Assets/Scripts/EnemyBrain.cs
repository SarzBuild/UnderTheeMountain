using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.AI;

public class EnemyBrain : MonoBehaviour
{
    [Header("Inventory")]
    public int MaxHealth = 100;
    public int CurrentHealth;
    public int ScoreToGive;
    
    [Header("References")]
    public Transform TargetPlayer;
    public NavMeshAgent NavMeshAgent;
    public Collider EnemyCollider;
    public Transform FleeToTarget;
    
    private PlayerReferences _playerReferences;
    
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

    [Header("Layer Masks")]
    public LayerMask PlayerLayerMask;
    public LayerMask FriendLayerMask;
    
    [Header("Detection Lists")]
    public Collider[] Colliders;
    public List<Transform> NearestTransforms = new List<Transform>();
    
    private void Awake()
    {
        EnemyCollider = GetComponent<Collider>();
        CurrentHealth = MaxHealth;
        _canMove = true;
    }

    private void Start()
    {
        _playerReferences = PlayerReferences.Instance;
    }

    private void Update()
    {
        HandleLookAtChooseTarget();
        HandleDeath();
        HandleMovement();
        HandleFlee();
        HandleAttack();
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
        if (Colliders != null && Colliders.Length > 0)
        {
            return true;
        }
        return false;
    }
    private bool HandleNearDetectionFriends(float radius)
    {
        Colliders = Physics.OverlapSphere(transform.position, radius, FriendLayerMask);
        if (Colliders != null && Colliders.Length > 0)
        {
            foreach (var vcollider in Colliders)
            {
                if (vcollider != EnemyCollider)
                {
                    if (!NearestTransforms.Contains(vcollider.transform))
                        NearestTransforms.Add(vcollider.transform);
                }
            }
            return true;
        }
        return false;
    }

    private void HandleLookAtChooseTarget()
    {
        if (HandleSeeingDistancePlayer())
        {
           HandleLookAt(TargetPlayer);
        }
        else if (HandleSeeingDistanceFriend())
        {
            HandleLookAt(HandleFindNearestFriend());
        }
    }

    private Transform HandleFindNearestFriend()
    {
        _nearestTarget = null;
        if (NearestTransforms != null && NearestTransforms.Count > 0)
        {
            float closestDistanceSqr = Mathf.Infinity;
            foreach (var possibleTarget in NearestTransforms)
            {
                Vector3 directionToTarget = possibleTarget.position - transform.position;
                float dSqrToTarget = directionToTarget.sqrMagnitude;
                if (dSqrToTarget < closestDistanceSqr)
                {
                    closestDistanceSqr = dSqrToTarget;
                    _nearestTarget = possibleTarget;
                }
            }
            return _nearestTarget;
        }
        return transform;
    }
    
    private void HandleLookAt(Transform target)
    {
        var angleOffset = 90f;
        
        _aimDirection = (target.position - transform.position).normalized;
        float angle = Mathf.Atan2(_aimDirection.x, _aimDirection.z) * Mathf.Rad2Deg;
        _rotationNeeded = new Vector3(0, angle - angleOffset, 0);
        transform.eulerAngles = _rotationNeeded;
        
    }

    private void HandleMovement()
    {
        if (CurrentHealth > RunThreshold || _possibleGangUp)
        {
            if (HandleAggroDistancePlayer())
            {
                if(NearestTransforms !=null && NearestTransforms.Count > 0)
                    foreach (var friend in NearestTransforms)
                        friend.GetComponent<EnemyBrain>()._attackPlayer = true;
                _attackPlayer = true;
            }
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
        if (!(NearestTransforms.Count > 2))
        {
            if (CurrentHealth < RunThreshold)
            {
                _attackPlayer = false;
                if (_canMove)
                {
                    NavMeshAgent.SetDestination(FleeToTarget.position);
                }
            }
        }
    }

    private void OnDisable()
    {
        OnDisableUnsubToFriendsLists();
    }
    private void OnDisableUnsubToFriendsLists()
    {
        foreach (var variTransform in NearestTransforms)
        {
            variTransform.GetComponent<EnemyBrain>().NearestTransforms.Remove(transform);
        }
    }

    public void GetDamaged(int damageAmount)
    {
        CurrentHealth -= damageAmount;
    }

    private void HandleDeath()
    {
        if (CurrentHealth <= 0)
        {
            _canMove = false;
            NavMeshAgent.isStopped = true;
            _playerReferences.ScoreCount += ScoreToGive;
            gameObject.SetActive(false);
        }
    }
}
