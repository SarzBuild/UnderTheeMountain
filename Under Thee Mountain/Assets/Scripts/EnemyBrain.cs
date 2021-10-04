using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.AI;

public class EnemyBrain : MonoBehaviour
{
    public Transform TargetPlayer;
    private Vector3 _movementVector;
    public float AggroRange;
    public int RunThreshold;
    public float SeeingRange;
    private Vector3 _originalPosition;
    private Vector3 _aimDirection;
    private Vector3 _rotationNeeded;
    public Collider EnemyCollider;
    private Transform _nearestTarget;

    private bool _possibleGangUp;
    private bool _attackPlayer;

    public int MaxHealth = 100;
    public int CurrentHealth;

    public Collider[] Colliders;
    public List<Transform> NearestTransforms = new List<Transform>();
    
    public LayerMask PlayerLayerMask;
    public LayerMask FriendLayerMask;

    public NavMeshAgent NavMeshAgent;

    public Transform EndLevelTransform;
    private bool _canMove;
    

    private void Awake()
    {
        EnemyCollider = GetComponent<Collider>();
        CurrentHealth = MaxHealth;
        _canMove = true;
    }

    private void Update()
    {
        HandleLookAtTarget();
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

    private void HandleLookAtTarget()
    {
        if (HandleSeeingDistancePlayer())
        {
           HandleLookAt(TargetPlayer);
        }
        else if (HandleSeeingDistanceFriend())
        {
            HandleLookAt(HandleLookAtNearestFriend());
        }
    }

    private Transform HandleLookAtNearestFriend()
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

    private void SendMessageToFriends()
    {
        foreach (var variTransform in NearestTransforms)
        {
            variTransform.GetComponent<EnemyBrain>().NearestTransforms.Remove(transform);
        }
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
        if (CurrentHealth > RunThreshold && !_possibleGangUp)
        {
            if (HandleAggroDistancePlayer())
            {
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
        {
            if (_attackPlayer)
            {
                NavMeshAgent.SetDestination(TargetPlayer.position);
            }
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
                    NavMeshAgent.SetDestination(EndLevelTransform.position);
                }
            }
        }
    }

    private void OnDisable()
    {
        SendMessageToFriends();
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
            gameObject.SetActive(false);
        }
    }
}
