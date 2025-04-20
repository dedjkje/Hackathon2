using TMPro;
using UnityEngine;

public class DoorScript : MonoBehaviour
{
    [SerializeField] SimpleButton button;
    [SerializeField] Transform DoorUp;
    [SerializeField] Transform DoorDown;
    [SerializeField] float openSpeed;
    void Start()
    {
        
    }
    
    void Update()
    {
        if (button != null)
        {
            if (button.isPressed)
            {
                transform.position = Vector3.MoveTowards(transform.position, DoorUp.position, openSpeed * Time.deltaTime);
            }
            if (!button.isPressed)
            {
                transform.position = Vector3.MoveTowards(transform.position, DoorDown.position, openSpeed * Time.deltaTime);
            }
        }
        else
        {
            transform.position = Vector3.MoveTowards(transform.position, DoorDown.position, openSpeed * Time.deltaTime);
        }
    }
    public void Transaction()
    {
        button = null;
    }
    public void BrainSwap()
    {
        Transform temp = DoorDown;
        DoorDown = DoorUp;
        DoorUp = temp;
    }
}
