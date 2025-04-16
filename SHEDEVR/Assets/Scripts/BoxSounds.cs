using UnityEngine;

public class BoxSounds : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip clip;
    public Settings settings;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.relativeVelocity.x + collision.relativeVelocity.y + collision.relativeVelocity.z > 0.2f)
        {
            audioSource.PlayOneShot(clip, settings.volume);
        }
    }
}
