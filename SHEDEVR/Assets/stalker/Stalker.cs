using UnityEngine;
using System.Collections;
using System.Xml.Serialization;

public class Stalker : MonoBehaviour
{
    public Settings settings;
    public ChangeGravity changeGravity;
    public float tempSpeed;

    [Header("Default Wall")]
    public string defaultWall;
    [Header("Waypoints")]
    public Transform[] waypoints;

    [Header("Movement Settings")]
    public float moveSpeed = 3f;
    public float rotationSpeed = 10f;
    public float stoppingDistance = 1f;
    public float slowDownDistance = 3f;
    public float minMoveThreshold = 0.2f;
    public float waitTime = 1f;
    public float obstacleAvoidDistance = 1.5f;
    public float obstacleAvoidForce = 2f;

    [Header("Position Stabilization")]
    public float positionStabilizationSpeed = 5f;
    public float maxWallDistance = 0.2f;

    [Header("Player Interaction")]
    public float playerDetectionRadius = 3f;
    public float stopFollowDistance = 5f;
    public float maxDistanceFromWaypoint = 7f;
    public float attackDistance = 0.5f;
    public float attackCooldown = 2f;
    public int attackDamage = 10;
    public LayerMask obstacleLayer;

    [Header("Wall Settings")]
    public LayerMask wallLayers;
    public float wallCheckDistance = 0.5f;
    public float rotationToWallSpeed = 5f;

    [Header("Model Settings")]
    public Transform modelTransform;
    public float modelRotationSpeed = 15f;

    [Header("Stalker")]
    public bool isDead = false;
    public GameObject Alive;
    public GameObject Dead;
    public AudioSource audioSource;
    public AudioClip deathAudio;

    // Private variables
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
    private Vector3 currentVelocity;
    private Transform localSpaceParent;
    public string currentWallTag;
    private Transform currentWall;
    private Quaternion targetWallRotation;
    private bool isRotatingToWall = false;
    private Vector3 localGroundOffset;
    private Vector3 lastWallPosition;
    private Quaternion lastWallRotation;

    void Start()
    {
        animator = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        playerController = player.GetComponent<FirstPersonController>();
        localSpaceParent = transform.parent;

        if (waypoints.Length > 0)
        {
            lastWaypointPosition = waypoints[currentWaypointIndex].position;
        }

        if (modelTransform == null && transform.childCount > 0)
        {
            modelTransform = transform.GetChild(0);
        }

        UpdateCurrentWall();
        tempSpeed = moveSpeed;
        CalculateGroundOffset();
    }

    void Update()
    {
        if (waypoints.Length == 0 || isDead) return;
        UpdateCurrentWall();
        HandleWallMovement();
        StabilizePosition();
        UpdateAttackState();
        UpdateDecisionMaking();
        UpdateMovement();
        UpdateAnimations();
        UpdateWallRotation();
        UpdateModelRotation();
    }

    private void CalculateGroundOffset()
    {
        if (currentWall != null)
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, -currentWall.up, out hit, wallCheckDistance * 2f, wallLayers))
            {
                localGroundOffset = transform.position - hit.point;
                lastWallPosition = currentWall.position;
                lastWallRotation = currentWall.rotation;
            }
        }
    }

    private void HandleWallMovement()
    {
        if (currentWall != null)
        {
            // Calculate movement of the wall since last frame
            Vector3 wallMovement = currentWall.position - lastWallPosition;
            Quaternion wallRotationChange = currentWall.rotation * Quaternion.Inverse(lastWallRotation);

            // Apply the same movement and rotation to the stalker
            transform.position += wallMovement;
            transform.rotation = wallRotationChange * transform.rotation;

            // Update last known wall position/rotation
            lastWallPosition = currentWall.position;
            lastWallRotation = currentWall.rotation;
        }
    }

    private void StabilizePosition()
    {
        if (currentWall == null) return;

        RaycastHit hit;
        if (Physics.Raycast(transform.position, -currentWall.up, out hit, maxWallDistance * 2f, wallLayers))
        {
            // Calculate desired position maintaining the local offset
            Vector3 desiredPosition = hit.point + localGroundOffset;

            // Smoothly adjust position
            transform.position = Vector3.Lerp(
                transform.position,
                desiredPosition,
                positionStabilizationSpeed * Time.deltaTime
            );

            // Constrain movement to wall plane
            Vector3 wallNormal = currentWall.up;
            Vector3 movementPlaneNormal = Vector3.Cross(transform.forward, wallNormal).normalized;
            Vector3 constrainedPosition = Vector3.ProjectOnPlane(transform.position, movementPlaneNormal);
            transform.position = new Vector3(constrainedPosition.x, transform.position.y, constrainedPosition.z);
        }
    }

    private void UpdateCurrentWall()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, -transform.up, out hit, wallCheckDistance, wallLayers))
        {
            if (hit.collider.transform != currentWall || hit.collider.tag != currentWallTag)
            {
                if (hit.collider != null) currentWallTag = hit.collider.tag;
                currentWall = hit.collider.transform;
                targetWallRotation = Quaternion.FromToRotation(transform.up, currentWall.up) * transform.rotation;
                isRotatingToWall = true;
                CalculateGroundOffset();
            }
        }
    }

    private void UpdateWallRotation()
    {
        if (isRotatingToWall && currentWall != null)
        {
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetWallRotation,
                rotationToWallSpeed * Time.deltaTime
            );

            if (Quaternion.Angle(transform.rotation, targetWallRotation) < 1f)
            {
                transform.rotation = targetWallRotation;
                isRotatingToWall = false;
            }
        }
    }

    private void UpdateModelRotation()
    {
        if (modelTransform == null || currentVelocity.magnitude < 0.1f || isAttacking || isRotatingToWall)
            return;

        Vector3 moveDirection = currentVelocity.normalized;
        if (moveDirection != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection, transform.up);
            modelTransform.rotation = Quaternion.Slerp(
                modelTransform.rotation,
                targetRotation,
                modelRotationSpeed * Time.deltaTime
            );
        }
    }

    private bool IsPlayerOnSameWall()
    {
        if (player == null) return false;

        PlayerWallDetector playerWallDetector = player.GetComponent<PlayerWallDetector>();
        if (playerWallDetector != null)
        {

            return playerWallDetector.CurrentWallTag == defaultWall;
        }

        RaycastHit playerHit;
        if (Physics.Raycast(player.position, -player.up, out playerHit, wallCheckDistance * 2, wallLayers))
        {
            return playerHit.collider.tag == currentWallTag;
        }

        return false;
    }

    private void UpdateAttackState()
    {
        if (isAttacking || !IsPlayerOnSameWall()) return;

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
        currentVelocity = Vector3.zero;
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
        bool playerInRange = distanceToPlayer <= playerDetectionRadius && IsPlayerOnSameWall();

        if (shouldReturnToPath)
        {
            if (distanceFromWaypoint <= stoppingDistance)
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
        if (isAttacking || isWaiting || isRotatingToWall)
        {
            currentVelocity = Vector3.zero;
            return;
        }

        Vector3 targetPosition = shouldReturnToPath ?
            lastWaypointPosition :
            (isFollowingPlayer ? player.position : waypoints[currentWaypointIndex].position);

        MoveToTarget(targetPosition);
    }

    private void MoveToTarget(Vector3 targetPosition)
    {
        Vector3 direction = targetPosition - transform.position;
        float distance = direction.magnitude;

        if (distance < stoppingDistance)
        {
            currentVelocity = Vector3.zero;
            if (!isFollowingPlayer && !shouldReturnToPath)
            {
                StartCoroutine(WaitAtWaypoint());
            }
            return;
        }

        direction = Vector3.ProjectOnPlane(direction, transform.up).normalized;

        float speed = moveSpeed;
        if (distance < slowDownDistance)
        {
            speed = moveSpeed * Mathf.Clamp01((distance - stoppingDistance) /
                                           (slowDownDistance - stoppingDistance));
        }

        Vector3 avoidForce = CalculateObstacleAvoidance();
        Vector3 finalDirection = (direction + avoidForce).normalized;

        if (distance > minMoveThreshold)
        {
            currentVelocity = finalDirection * speed;
            Vector3 newPosition = transform.position + currentVelocity * Time.deltaTime;

            // Project movement onto wall plane
            if (currentWall != null)
            {
                Vector3 wallNormal = currentWall.up;
                newPosition = Vector3.ProjectOnPlane(newPosition - currentWall.position, wallNormal) + currentWall.position;

                // Maintain distance from wall
                RaycastHit hit;
                if (Physics.Raycast(newPosition, -wallNormal, out hit, maxWallDistance * 2f, wallLayers))
                {
                    newPosition = hit.point + localGroundOffset;
                }
            }

            transform.position = newPosition;
        }
    }

    private Vector3 CalculateObstacleAvoidance()
    {
        Vector3 avoidForce = Vector3.zero;
        RaycastHit hit;

        Vector3 rayDirection = Vector3.ProjectOnPlane(transform.forward, transform.up).normalized;

        if (Physics.Raycast(transform.position, rayDirection, out hit, obstacleAvoidDistance, obstacleLayer))
        {
            avoidForce += hit.normal * obstacleAvoidForce;
        }

        Vector3[] rayDirections = new Vector3[] {
            rayDirection + transform.right,
            rayDirection - transform.right
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
        bool isMoving = currentVelocity.magnitude > 0.1f && !isAttacking;
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
        currentVelocity = Vector3.zero;
        yield return new WaitForSeconds(waitTime);

        currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
        lastWaypointPosition = waypoints[currentWaypointIndex].position;
        isWaiting = false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, stoppingDistance);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, slowDownDistance);

        Gizmos.color = Color.cyan;
        Vector3 rayDirection = Vector3.ProjectOnPlane(transform.forward, transform.up).normalized;
        Gizmos.DrawRay(transform.position, rayDirection * obstacleAvoidDistance);
        Gizmos.DrawRay(transform.position, (rayDirection + transform.right) * obstacleAvoidDistance * 0.7f);
        Gizmos.DrawRay(transform.position, (rayDirection - transform.right) * obstacleAvoidDistance * 0.7f);

        Gizmos.color = Color.green;
        Gizmos.DrawRay(transform.position, -transform.up * wallCheckDistance);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.GetComponent<CanKillStalker>() && collision.rigidbody.linearVelocity.magnitude > 0.8f)
        {
            if (!isDead)
            {
                enabled = false;
                Alive.SetActive(false);
                Dead.transform.parent = null;
                Dead.SetActive(true);
                audioSource.PlayOneShot(deathAudio, settings.volume);
                isDead = true;
            }
        }
    }
}