using Mono.Cecil;
using System.Diagnostics.Contracts;
using UnityEngine;

public class CastPull : MonoBehaviour
{
    [HideInInspector] public bool canPull;
    [HideInInspector] public bool PullStarted;
    [HideInInspector] public bool PullEnded;
    [HideInInspector] public bool animationEnded;
    [HideInInspector] public bool onTarget;
    [HideInInspector] public bool stopAnimation;

    [Header("Камера")]
    [SerializeField] Camera playerCamera;

    [Header("Ширина обводки")]
    [SerializeField] float width;

    [Header("Время притягивания")]
    [SerializeField] float time;

    private GameObject pullable;
    private bool canMove = false;
    private Vector3 distance;
    private Rigidbody rb;
    private Vector3 target;

    private float timer;

    void Start()
    {
        
        PullStarted = false;
        PullEnded = true;
        animationEnded = false;
    }

    private void FixedUpdate()
    {
        if (canMove) 
        {
            PullStarted = true;
            PullEnded = false;
            rb.constraints = RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY;
            rb.gameObject.GetComponent<Outline>().OutlineWidth = width;
            distance = new Vector3(
                target.x - rb.transform.position.x,
                rb.linearVelocity.y,
                target.z - rb.transform.position.z);
            rb.linearVelocity = distance / time;
            if (Vector3.Distance(target, rb.transform.position) < 2 || rb.linearVelocity.magnitude < 0.1f || rb.gameObject.GetComponent<PullableFlag>().playerCollision || Time.time - timer > 3f)
            {
                stopAnimation = true;
                PullEnded = true;
                PullStarted = false;
                rb.gameObject.GetComponent<Outline>().OutlineWidth = 0f;
                rb.linearVelocity = Vector3.zero;
                rb.constraints = RigidbodyConstraints.None;
                rb = null;
                canMove = false;
            }
        }
    }

    void Update()
    {
        if (canPull && animationEnded && PullEnded) {
            Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 100f))
            {
                if (hit.collider.gameObject.GetComponent<PullableFlag>())
                {
                    onTarget = true;
                    if (pullable != hit.collider.gameObject && pullable != null)
                    {
                        pullable.GetComponent<Outline>().OutlineWidth = 0f;
                    }
                    pullable = hit.collider.gameObject;
                    pullable.GetComponent<Outline>().OutlineWidth = width;
                }
                else if (pullable != null)
                {
                    pullable.GetComponent<Outline>().OutlineWidth = 0f;
                    pullable = null;
                }
                else
                {
                    onTarget = false;
                }
            }
        }
        else
        {
            if (pullable != null) {
                pullable.GetComponent<Outline>().OutlineWidth = 0f;
                pullable = null;
            }
        }
    }

    public void Pull()
    {
        stopAnimation = false;
        if (pullable != null && canPull && animationEnded)
        {
            rb = pullable.GetComponent<Rigidbody>();
            target = playerCamera.transform.position;
            canMove = true;
            timer = Time.time;
        }
    }
    public void End()
    {
        animationEnded = true;
    }
    public void NoEnd()
    {
        animationEnded = false;
    }
}
