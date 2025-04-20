using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class UpFlag : MonoBehaviour
{
    private Outline outline;
    private bool hasOutline;
    private float width;
    private Color color;
    public GameObject holder;
    private Rigidbody rb;
    private Vector3 distance;
    private bool backOutline;
    private Abilities abilities;
    private Vector3 originalScale;
    private bool isKinematic;
    private ChangeGravity changeGravity;
    private GameObject currentCilinder;
    private AudioSource source;
    [SerializeField] AudioClip cilinderEnter;
    private float soundInterval = 2f;
    private float lastPlayTime = 0;

    void Start()
    {
        source = GetComponent<AudioSource>();
        originalScale = transform.localScale;
        changeGravity = GameObject.Find("Player").transform.Find("Hand").gameObject.GetComponent<ChangeGravity>();
        abilities = GameObject.Find("Player").transform.Find("Hand").gameObject.GetComponent<Abilities>();
        holder = null;
        rb = GetComponent<Rigidbody>();
        outline = GetComponent<Outline>();
        if (outline != null)
        {
            width = outline.OutlineWidth;
            color = outline.OutlineColor;
        }
    }

    void Update()
    {
        if (changeGravity.isRotating && currentCilinder != null)
        {
            transform.SetParent(currentCilinder.transform, true);
            rb.isKinematic = true;
        }
        if (!changeGravity.isRotating && currentCilinder != null)
        {
            transform.SetParent(null, true);
            rb.isKinematic = false;
        }
        if (GameObject.Find("Player").transform.Find("Hand").gameObject.GetComponent<Abilities>().changing)
        {
            outline.OutlineWidth = 0;
        }
        if (holder != null)
        {
            distance = new Vector3(
                holder.transform.position.x / 1.2f - transform.position.x,
                holder.transform.position.y / 1.2f - transform.position.y,
                holder.transform.position.z / 1.2f - transform.position.z);
            rb.linearVelocity = distance;
        }
        else
        {
            if (!GameObject.Find("Player").transform.Find("Hand").gameObject.GetComponent<ChangeGravity>().isRotating)
            {
                rb.useGravity = true;
                if (backOutline && abilities.currentAbility == Abilities.Ability.AddGravity)
                {
                    outline.OutlineWidth = width;
                    outline.OutlineColor = color;
                }
                if (abilities.currentAbility == Abilities.Ability.PullObject)
                {
                    outline.OutlineColor = new Color(1f, 180f / 250f, 0f, 1f);
                }
            }
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "predict")
        {
            backOutline = false;
            if (GetComponent<Outline>())
            {
                hasOutline = true;
                outline.OutlineWidth = 7f;
                outline.OutlineColor = new Color(0f, 180f / 255f, 1f, 1f);
            }
            else
            {
                outline = gameObject.AddComponent<Outline>();
                outline.OutlineWidth = 7f;
                outline.OutlineColor = new Color(0f, 180f / 255f, 1f, 1f);
                hasOutline = false;
            }
        }
        if (other.tag == "cilinder")
        {
            if (Time.time - lastPlayTime >= soundInterval)
            {
                source.clip = cilinderEnter;
                source.Play();
                lastPlayTime = Time.time;
                source.clip = cilinderEnter;
            }
            currentCilinder = other.gameObject;
            holder = other.gameObject.transform.parent.transform.Find("Holder").gameObject;
            rb.useGravity = false;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "predict")
        {
            backOutline = true;
            if (hasOutline)
            {
                outline.OutlineWidth = width;
                outline.OutlineColor = color;
            }
            else
            {
                Destroy(outline);
            }
        }
        if (other.tag == "cilinder")
        {
            currentCilinder = null;
            rb.useGravity = true;

            holder = null;
        }
    }
}
