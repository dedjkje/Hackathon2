using UnityEngine;

public class PullableFlag : MonoBehaviour
{
    [HideInInspector] public bool playerCollision;
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        playerCollision = false;
    }

    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.GetComponent<CharacterController>())
        {
            playerCollision = true;
        }
    }
    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.GetComponent<CharacterController>())
        {
            playerCollision = true;
        }
    }
    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.GetComponent<CharacterController>())
        {
            playerCollision = false;
        }
    }
}
