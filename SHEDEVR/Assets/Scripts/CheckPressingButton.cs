using UnityEngine;

public class CheckPressingButton : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public bool inTrigger = false;
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag != "predict" && other.tag != "button" && other.gameObject.layer != 8)
        {
            inTrigger = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.tag != "predict" && other.tag != "button" && other.gameObject.layer != 8)
        {
            inTrigger = false;
        }
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
