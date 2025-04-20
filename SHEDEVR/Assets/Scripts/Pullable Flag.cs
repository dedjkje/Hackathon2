using UnityEngine;
using UnityEngine.Rendering;

public class PullableFlag : MonoBehaviour
{
    [HideInInspector] public bool playerCollision;
    private Rigidbody rb;
    public bool stop;

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
        if (collision.gameObject.tag == "spike")
        {
                Debug.Log("wewewe");
                stop = true;
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
