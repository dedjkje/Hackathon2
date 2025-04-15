using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class UpFlag : MonoBehaviour
{
    private Outline outline;
    private bool hasOutline;
    private float width;
    private Color color;
    private GameObject holder;
    private Rigidbody rb;
    private Vector3 distance;
    private bool backOutline;
    private Abilities abilities;

    void Start()
    {
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
            holder = other.gameObject.transform.Find("Holder").gameObject;
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
            holder = null;
            rb.useGravity = true;
        }
    }
}
