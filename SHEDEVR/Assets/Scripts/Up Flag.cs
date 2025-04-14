using UnityEngine;

public class UpFlag : MonoBehaviour
{
    private Outline outline;
    private bool hasOutline;
    private float width;
    private Color color;

    void Start()
    {
        
    }

    void Update()
    {
        
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "predict")
        {
            if (GetComponent<Outline>())
            {
                hasOutline = true;
                outline = GetComponent<Outline>();
                width = outline.OutlineWidth;
                color = outline.OutlineColor;
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
    }
    private void OnTriggerStay(Collider other)
    {
        outline.OutlineWidth = 7f;
        outline.OutlineColor = new Color(0f, 180f / 255f, 1f, 1f);
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "predict")
        {
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
    }
}
