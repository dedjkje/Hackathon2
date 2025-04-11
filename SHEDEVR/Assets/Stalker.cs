using UnityEngine;
using System.Collections;

public class Stalker : MonoBehaviour
{
    [Header("Waypoints")]
    public Transform[] waypoints;

    [Header("Movement Settings")]
    public float moveSpeed = 3f;
    public float rotationSpeed = 12f;
    public float stoppingDistance = 0.5f;
    public float slowDownDistance = 2f;
    public float waitTime = 1f;
    public float obstacleAvoidDistance = 1.5f;
    public float obstacleAvoidForce = 2f;

    [Header("Player Interaction")]
    public float playerDetectionRadius = 3f;
    public float stopFollowDistance = 5f;
    public float maxDistanceFromWaypoint = 7f;
    public float attackDistance = 0.5f;
    public float attackCooldown = 2f;
    public int attackDamage = 10;
    public LayerMask obstacleLayer;

    private Rigidbody rb;
    private Animator animator;
    private Transform player;
    private FirstPersonController playerController;
    private int currentWaypointIndex = 0;
    private bool isWaiting = false;
    private bool isFollowingPlayer = false;
    private bool isAttacking = false;
    private bool shouldReturnToPath = false;
    private float lastAttackTime = 0f;
    private Vector3 lastWaypointPosition;
    private Vector3 currentAvoidDirection;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        playerController = player.GetComponent<FirstPersonController>();

        if (waypoints.Length > 0)
        {
            lastWaypointPosition = waypoints[currentWaypointIndex].position;
        }

        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
    }

    void Update()
    {
        if (waypoints.Length == 0) return;

        UpdateAttackState();
        UpdateDecisionMaking();
        UpdateMovement();
        UpdateAnimations();
    }

    private void UpdateAttackState()
    {
        if (isAttacking) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        if (distanceToPlayer <= attackDistance && playerController != null)
        {
            if (Time.time - lastAttackTime >= attackCooldown)
            {
                StartCoroutine(PerformAttack());
            }
        }
    }

    private IEnumerator PerformAttack()
    {
        isAttacking = true;
        rb.linearVelocity = Vector3.zero;
        animator.SetBool("isAttacking", true);
        lastAttackTime = Time.time;

        if (playerController != null)
        {
            playerController.hp -= attackDamage;
        }

        yield return new WaitForSeconds(1f);
        animator.SetBool("isAttacking", false);
        isAttacking = false;
    }

    private void UpdateDecisionMaking()
    {
        if (isAttacking) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        float distanceFromWaypoint = Vector3.Distance(transform.position, lastWaypointPosition);
        bool playerInRange = distanceToPlayer <= playerDetectionRadius;

        if (shouldReturnToPath)
        {
            if (distanceFromWaypoint <= 0.5f)
            {
                shouldReturnToPath = false;
                isFollowingPlayer = false;
            }
            return;
        }

        if (playerInRange && playerController != null)
        {
            if (!isFollowingPlayer && distanceFromWaypoint <= maxDistanceFromWaypoint)
            {
                StartFollowingPlayer();
            }
            else if (isFollowingPlayer &&
                    (distanceToPlayer > stopFollowDistance ||
                     distanceFromWaypoint > maxDistanceFromWaypoint))
            {
                InitiateReturnToPath();
            }
        }
    }

    private void InitiateReturnToPath()
    {
        shouldReturnToPath = true;
        isFollowingPlayer = false;
        currentWaypointIndex = GetNearestWaypointIndex();
        lastWaypointPosition = waypoints[currentWaypointIndex].position;
    }

    private void UpdateMovement()
    {
        if (isAttacking || isWaiting)
        {
            rb.linearVelocity = Vector3.zero;
            return;
        }

        Vector3 targetPosition = shouldReturnToPath ?
            lastWaypointPosition :
            (isFollowingPlayer ? player.position : waypoints[currentWaypointIndex].position);

        float distanceToTarget = Vector3.Distance(transform.position, targetPosition);

        if (distanceToTarget > stoppingDistance)
        {
            MoveToTarget(targetPosition, distanceToTarget);
        }
        else if (!isFollowingPlayer && !shouldReturnToPath)
        {
            StartCoroutine(WaitAtWaypoint());
        }
    }

    private void MoveToTarget(Vector3 targetPosition, float currentDistance)
    {
        Vector3 directionToTarget = (targetPosition - transform.position).normalized;
        directionToTarget.y = 0;

        Vector3 avoidForce = CalculateObstacleAvoidance();
        Vector3 finalDirection = (directionToTarget + avoidForce).normalized;

        if (finalDirection != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(finalDirection);
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );
        }

        float speedMultiplier = currentDistance < slowDownDistance ?
            Mathf.Clamp01(currentDistance / slowDownDistance) : 1f;

        rb.linearVelocity = transform.forward * moveSpeed * speedMultiplier;
    }

    private Vector3 CalculateObstacleAvoidance()
    {
        Vector3 avoidForce = Vector3.zero;
        RaycastHit hit;

        if (Physics.Raycast(transform.position, transform.forward, out hit, obstacleAvoidDistance, obstacleLayer))
        {
            avoidForce += hit.normal * obstacleAvoidForce;
        }

        Vector3[] rayDirections = new Vector3[] {
            transform.forward + transform.right,
            transform.forward - transform.right
        };

        foreach (Vector3 dir in rayDirections)
        {
            if (Physics.Raycast(transform.position, dir, out hit, obstacleAvoidDistance * 0.7f, obstacleLayer))
            {
                avoidForce += hit.normal * obstacleAvoidForce * 0.5f;
            }
        }

        return avoidForce;
    }

    private void UpdateAnimations()
    {
        bool isMoving = rb.linearVelocity.magnitude > 0.1f && !isAttacking;
        animator.SetBool("isWalking", isMoving);
    }

    private void StartFollowingPlayer()
    {
        isFollowingPlayer = true;
        isWaiting = false;
        shouldReturnToPath = false;
        StopAllCoroutines();
    }

    private int GetNearestWaypointIndex()
    {
        int nearestIndex = 0;
        float minDistance = float.MaxValue;

        for (int i = 0; i < waypoints.Length; i++)
        {
            float distance = Vector3.Distance(transform.position, waypoints[i].position);
            if (distance < minDistance)
            {
                minDistance = distance;
                nearestIndex = i;
            }
        }

        return nearestIndex;
    }

    private IEnumerator WaitAtWaypoint()
    {
        isWaiting = true;
        rb.linearVelocity = Vector3.zero;
        yield return new WaitForSeconds(waitTime);

        currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
        lastWaypointPosition = waypoints[currentWaypointIndex].position;
        isWaiting = false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawRay(transform.position, transform.forward * obstacleAvoidDistance);
        Gizmos.DrawRay(transform.position, (transform.forward + transform.right) * obstacleAvoidDistance * 0.7f);
        Gizmos.DrawRay(transform.position, (transform.forward - transform.right) * obstacleAvoidDistance * 0.7f);
    }
}