using TMPro;
using UnityEngine;

public class DoorScript : MonoBehaviour
{
    [SerializeField] SimpleButton button;
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
        if (movementBegan)
        {
            playSound();
        }
        if (button != null)
        {
            if (button.isPressed)
            {
                if (!lastWasOpenning)
                {
                    movementBegan = true;
                    lastWasOpenning = true;
                }
                transform.position = Vector3.MoveTowards(transform.position, DoorUp.position, openSpeed * Time.deltaTime * 1.5f);
            }
            if (!button.isPressed)
            {
                if (lastWasOpenning)
                {
                    movementBegan = true;
                    lastWasOpenning = false;
                }
                transform.position = Vector3.MoveTowards(transform.position, DoorDown.position, openSpeed * Time.deltaTime * 1.5f);
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
    public void Transaction()
    {
        playSound();
        button = null;
    }
    public void playSound()
    {
        if (source != null)
        {
            source.Play();
            movementBegan = false;
        }
    }
    public void BrainSwap()
    {
        playSound();
        Transform temp = DoorDown;
        DoorDown = DoorUp;
        DoorUp = temp;
    }
}
