using UnityEngine;

public class freezeWolf2 : MonoBehaviour
{
    void Start()
    {
        
    }

    
    void Update()
    {
        transform.rotation = Quaternion.Euler(0f, transform.rotation.eulerAngles.y, 0f);
    }
}
