using TMPro;
using UnityEngine;
using static Unity.VisualScripting.Member;

public class DoorScript1 : MonoBehaviour
{
    [SerializeField] SimpleButton button1;
    [SerializeField] SimpleButton button2;
    [SerializeField] Transform DoorUp;
    [SerializeField] Transform DoorDown;
    [SerializeField] float openSpeed;
    private bool movementBegan = false;
    private bool lastWasOpenning = false;
    private AudioSource source;
    void Start()
    {
        source = GetComponent<AudioSource>();
    }
    
    void Update()
    {
        if (button1 != null && button2 != null)
        {
            if (button1.isPressed && button2.isPressed)
            {
                if (!lastWasOpenning)
                {
                    movementBegan = true;
                    lastWasOpenning = true;
                }
                transform.position = Vector3.MoveTowards(transform.position, DoorUp.position, openSpeed * Time.deltaTime * 1.5f);
            }
            if (!button1.isPressed || !button2.isPressed)
            {
                if (lastWasOpenning)
                {
                    movementBegan = true;
                    lastWasOpenning = false;
                }
                transform.position = Vector3.MoveTowards(transform.position, DoorDown.position, openSpeed * Time.deltaTime* 1.5f);
            }
        }
        else
        {
            if (lastWasOpenning)
            {
                movementBegan = true;
                lastWasOpenning = false;
            }
            transform.position = Vector3.MoveTowards(transform.position, DoorDown.position, openSpeed * Time.deltaTime * 1.5f);
        }
    }
    public void playSound()
    {
        if (source != null)
        {
            source.Play();
            movementBegan = false;
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
