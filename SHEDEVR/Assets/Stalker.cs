using UnityEngine;
using System.Collections;

public class Stalker : MonoBehaviour
{
    [Header("Waypoints")]
    public Transform[] waypoints;

    [Header("Movement Settings")]
    public float moveSpeed = 3f;
    public float rotationSpeed = 10f;
    public float stoppingDistance = 0.5f;
    public float slowDownDistance = 2f;
    public float waitTime = 1f;

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
    private float lastAttackTime = 0f;
    private Vector3 lastWaypointPosition;

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

        // Обновление состояний
        UpdateAttackState();
        UpdatePlayerFollowing();
        UpdateMovement();

        // Обновление анимаций
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

        // Нанесение урона
        if (playerController != null)
        {
            playerController.hp -= attackDamage;
            Debug.Log($"Dog attacked! Player HP: {playerController.hp}");
        }

        // Ждем завершения анимации атаки
        yield return new WaitForSeconds(1f);
        animator.SetBool("isAttacking", false);
        isAttacking = false;
    }

    private void UpdatePlayerFollowing()
    {
        if (isAttacking) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        bool playerInRange = distanceToPlayer <= playerDetectionRadius;

        if (playerInRange && playerController != null)
        {
            float distanceFromWaypoint = Vector3.Distance(transform.position, lastWaypointPosition);

            if (!isFollowingPlayer && distanceFromWaypoint <= maxDistanceFromWaypoint)
            {
                StartFollowingPlayer();
            }
            else if (isFollowingPlayer &&
                    (distanceToPlayer > stopFollowDistance ||
                     distanceFromWaypoint > maxDistanceFromWaypoint))
            {
                StopFollowingPlayer();
            }
        }
    }

    private void UpdateMovement()
    {
        if (isAttacking || isWaiting) return;

        Vector3 targetPosition = isFollowingPlayer ? player.position : waypoints[currentWaypointIndex].position;
        float distanceToTarget = Vector3.Distance(transform.position, targetPosition);

        if (distanceToTarget > stoppingDistance)
        {
            MoveToTarget(targetPosition, distanceToTarget);
        }
        else if (!isFollowingPlayer)
        {
            StartCoroutine(WaitAtWaypoint());
        }
    }

    private void MoveToTarget(Vector3 targetPosition, float currentDistance)
    {
        Vector3 directionToTarget = (targetPosition - transform.position).normalized;
        directionToTarget.y = 0;

        if (directionToTarget != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );
        }

        float angleToTarget = Vector3.Angle(transform.forward, directionToTarget);
        bool isFacingTarget = angleToTarget < 30f;

        if (isFacingTarget)
        {
            float speedMultiplier = currentDistance < slowDownDistance ?
                Mathf.Clamp01(currentDistance / slowDownDistance) : 1f;

            if (!Physics.Raycast(transform.position, transform.forward, 1f, obstacleLayer))
            {
                rb.linearVelocity = transform.forward * moveSpeed * speedMultiplier;
            }
            else
            {
                Vector3 avoidDirection = Vector3.Cross(Vector3.up, transform.forward).normalized;
                rb.linearVelocity = avoidDirection * moveSpeed * 0.5f;
            }
        }
        else
        {
            rb.linearVelocity = Vector3.zero;
        }
    }

    private void UpdateAnimations()
    {
        bool isMoving = rb.linearVelocity.magnitude > 0.1f && !isAttacking;
        animator.SetBool("isWalking", isMoving);

        // isAttacking уже управляется в PerformAttack
    }

    private void StartFollowingPlayer()
    {
        isFollowingPlayer = true;
        isWaiting = false;
        StopAllCoroutines();
    }

    private void StopFollowingPlayer()
    {
        isFollowingPlayer = false;
        currentWaypointIndex = GetNearestWaypointIndex();
        lastWaypointPosition = waypoints[currentWaypointIndex].position;
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
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, playerDetectionRadius);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackDistance);

        if (waypoints != null && waypoints.Length > 0)
        {
            Gizmos.color = Color.blue;
            for (int i = 0; i < waypoints.Length; i++)
            {
                if (waypoints[i] != null)
                {
                    Gizmos.DrawSphere(waypoints[i].position, 0.2f);
                    if (i < waypoints.Length - 1 && waypoints[i + 1] != null)
                    {
                        Gizmos.DrawLine(waypoints[i].position, waypoints[i + 1].position);
                    }
                }
            }
        }
    }
}