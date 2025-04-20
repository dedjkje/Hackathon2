using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UIElements;
using static Unity.VisualScripting.Member;
using static UnityEngine.GraphicsBuffer;

public class UpMonster : MonoBehaviour
{
    public GameObject holder;
    private Rigidbody rb;
    private Vector3 distance;
    private Abilities abilities;
    private Vector3 originalScale;
    private bool isKinematic;
    private ChangeGravity changeGravity;
    private GameObject currentCilinder;
    private Stalker stalker;
    private AudioSource source;
    [SerializeField] AudioClip cilinderEnter;
    private float soundInterval = 2f;
    private float lastPlayTime = 0;
    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
        source = GetComponent<AudioSource>();
        stalker = GetComponent<Stalker>();
        rb = GetComponent<Rigidbody>();
        originalScale = transform.localScale;
        changeGravity = GameObject.Find("Player").transform.Find("Hand").gameObject.GetComponent<ChangeGravity>();
        abilities = GameObject.Find("Player").transform.Find("Hand").gameObject.GetComponent<Abilities>();
        holder = null;
    }

    void Update()
    {
        if (rb != null)
        {
            if (changeGravity.isRotating && currentCilinder != null)
            {
                transform.SetParent(currentCilinder.transform, true);
            }
            if (!changeGravity.isRotating && currentCilinder != null)
            {
                transform.SetParent(null, true);
            }
            if (holder != null)
            {
                distance = new Vector3(
                    holder.transform.position.x - transform.position.x,
                    holder.transform.position.y - transform.position.y,
                    holder.transform.position.z - transform.position.z);
                rb.linearVelocity = distance;
            }
            else
            {
                if (!GameObject.Find("Player").transform.Find("Hand").gameObject.GetComponent<ChangeGravity>().isRotating)
                {
                    rb.useGravity = true;
                }
            }
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "cilinder")
        {
            if (Time.time - lastPlayTime >= soundInterval)
            {
                if (!changeGravity.isRotating)
                {
                    source.clip = cilinderEnter;
                    source.Play();
                    lastPlayTime = Time.time;
                    source.clip = cilinderEnter;
                }
            }
            stalker.enabled = false;
            currentCilinder = other.gameObject;
            holder = other.gameObject.transform.parent.transform.Find("Holder Extra").gameObject;
            rb.useGravity = false;
            rb.isKinematic = false;
            animator.speed = 0.2f;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "cilinder")
        {
            currentCilinder = null;
            rb.useGravity = true;
            holder = null;
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == 8)
        { // Walls
            animator.speed = 1f;
            rb.isKinematic = true;
            rb.useGravity = false;
            if (stalker.defaultWall != collision.gameObject.tag)
            {
                //stalker.enabled = false;
                //stalker.Alive.SetActive(false);
                //stalker.Dead.transform.parent = null;
                //stalker.Dead.SetActive(true);
                stalker.audioSource.PlayOneShot(stalker.deathAudio, stalker.settings.volume);
                stalker.isDead = true;
                stalker.Death();
            }
            else
            {
                stalker.enabled = true;
            }
        }
    }
}