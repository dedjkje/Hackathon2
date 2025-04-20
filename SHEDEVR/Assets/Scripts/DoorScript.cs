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
        if (button.isPressed)
        {
            transform.position = Vector3.MoveTowards(transform.position, DoorUp.position, openSpeed * Time.deltaTime);
        }
        if (!button.isPressed)
        {
            transform.position = Vector3.MoveTowards(transform.position, DoorDown.position, openSpeed * Time.deltaTime);
        }
    }
}
