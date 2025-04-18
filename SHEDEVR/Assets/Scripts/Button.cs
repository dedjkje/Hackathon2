using System.Collections;
using UnityEngine;

public class SimpleButton : MonoBehaviour
{
    public Transform defaultPos;
    public Transform pressedPos;
    public float pressSpeed = 2f;

    public bool isPressed = false;
    private Vector3 targetPosition;

    public CheckPressingButton checkPressingButton;

    void Start()
    {
        defaultPos = transform.parent.Find("StateDefault");
        pressedPos = transform.parent.Find("StatePressed");
    }

    void Update()
    {
        targetPosition = isPressed ? pressedPos.position : defaultPos.position;
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, pressSpeed * Time.deltaTime);

        if (!checkPressingButton.inTrigger)
        {
            isPressed = false;
        }
    }

    private void OnCollisionStay(Collision other)
    {
        if (other.transform.tag != "predict" && other.transform.tag != "button" && other.gameObject.layer != 8) isPressed = true;
    }
    

    private void OnCollisionExit(Collision other)
    {
        if (other.transform.tag != "predict" && other.transform.tag != "button" && other.gameObject.layer != 8 && !checkPressingButton.inTrigger) isPressed = false;
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "playerTrigger") isPressed = true;
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "playerTrigger") isPressed = false;
    }
}