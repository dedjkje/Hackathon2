using TMPro;
using UnityEngine;

public class DoorScript1 : MonoBehaviour
{
    [SerializeField] SimpleButton button1;
    [SerializeField] SimpleButton button2;
    [SerializeField] Transform DoorUp;
    [SerializeField] Transform DoorDown;
    [SerializeField] float openSpeed;
    void Start()
    {
        
    }
    
    void Update()
    {
        if (button1 != null && button2 != null)
        {
            if (button1.isPressed && button2.isPressed)
            {
                transform.position = Vector3.MoveTowards(transform.position, DoorUp.position, openSpeed * Time.deltaTime);
            }
            if (!button1.isPressed || !button2.isPressed)
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
        button1 = null;
        button2 = null;
    }
    public void BrainSwap()
    {
        Transform temp = DoorDown;
        DoorDown = DoorUp;
        DoorUp = temp;
    }
}
