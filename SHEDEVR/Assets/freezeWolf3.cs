using UnityEngine;

public class freezeWolf3 : MonoBehaviour
{
    
    void Start()
    {
        
    }

    
    void Update()
    {
        transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, -180, -90);
    }
}
