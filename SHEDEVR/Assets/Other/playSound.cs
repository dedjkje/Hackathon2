using UnityEngine;

public class playSound : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter(Collider collision)
    {
        if (collision.tag == "playerTrigger")
        {
            GetComponent<AudioSource>().Play(); 
        }
    }
}
