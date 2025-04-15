using Mono.Cecil;
using System.Diagnostics.Contracts;
using UnityEngine;
using System;

public class CastPull : MonoBehaviour
{
    [HideInInspector] public bool canPull;
    [HideInInspector] public bool PullStarted;
     public bool PullEnded;
    [HideInInspector] public bool animationEnded;
    [HideInInspector] public bool onTarget;
    [HideInInspector] public bool stopAnimation;

    [Header("Камера")]
    [SerializeField] Camera playerCamera;

    [Header("Ширина обводки")]
    [SerializeField] float width;

    [Header("Время притягивания")]
    [SerializeField] float time;

    [Header("Сквозь что проходит луч")]
    [SerializeField] LayerMask layerMask;

    [HideInInspector] public GameObject pullable;
    private bool canMove = false;
    private Vector3 distance;
    private Rigidbody rb;
    private Vector3 target;
    public int staticCount;
    private Abilities abilities;
    private float timer;
    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
        abilities = GetComponent<Abilities>();
        staticCount = 0;
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
            rb.gameObject.GetComponent<Outline>().OutlineWidth = width;
            rb.gameObject.GetComponent<Outline>().OutlineColor = new Color(1f, 180f/250f, 0f, 1f);
            distance = new Vector3(
                target.x - rb.transform.position.x,
                rb.linearVelocity.y,
                target.z - rb.transform.position.z);
            
            rb.linearVelocity = distance / time;
            if ((Vector3.Distance(target, rb.transform.position) < 2)|| rb.linearVelocity.magnitude < 0.1f || rb.gameObject.GetComponent<PullableFlag>().playerCollision || (Time.time - timer > 3f))
            { 
                stopAnimation = true;
                PullEnded = true;
                PullStarted = false;
                rb.gameObject.GetComponent<Outline>().OutlineWidth = 0f;
                rb.linearVelocity = new Vector3(0, rb.linearVelocity.y, 0);
                rb.constraints = RigidbodyConstraints.None;
                rb = null;
                canMove = false;
            }
        }
    }

    void Update()
    {
        if (canPull && animationEnded && PullEnded && !abilities.changing && !animator.GetBool("change")) {
            Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 100f, layerMask) && PullEnded)
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
            onTarget = false;
            if (pullable != null)
            {
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
            staticCount = 0;
            rb = pullable.GetComponent<Rigidbody>();
            Debug.Log($"{Math.Abs(Math.Round(pullable.transform.eulerAngles.x)) % 90}, {Math.Abs(Math.Round(pullable.transform.eulerAngles.y)) % 90}, {Math.Abs(Math.Round(pullable.transform.eulerAngles.z)) % 90}");
            if (Math.Abs(Math.Round(pullable.transform.eulerAngles.x)) % 90 == 0) staticCount++;
            if (Math.Abs(Math.Round(pullable.transform.eulerAngles.y)) % 90 == 0) staticCount++;
            if (Math.Abs(Math.Round(pullable.transform.eulerAngles.z)) % 90 == 0) staticCount++;
            Debug.Log(staticCount);
            if (staticCount >= 2)
            {
                rb.constraints = RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY;
            }
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
