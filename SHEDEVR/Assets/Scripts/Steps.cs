using UnityEngine;
using UnityEngine.Rendering;

public class Steps : MonoBehaviour
{
    [Header("Звуки")]
    [SerializeField] AudioClip defaultStep;
    [SerializeField] AudioClip landing;

    [Header("Игрок")]
    [SerializeField] FirstPersonController player;

    [Header("Интервал шагов")]
    [SerializeField] float interval;

    private AudioSource source;
    private Vector2 currentSpeed;
    private float timeFromLastStep;
    private bool playLanding = false;

    void Start()
    {
        source = GetComponent<AudioSource>();
    }

    private void FixedUpdate()
    {
        currentSpeed = new Vector2(player.moveVector.x, player.moveVector.z);
    }

    void Update()
    {
        timeFromLastStep += Time.deltaTime * currentSpeed.magnitude;

        if (timeFromLastStep > interval && player.isGrounded)
        {
            source.resource = defaultStep;
            source.Play();
            timeFromLastStep = 0;
        }
        if (currentSpeed.magnitude == 0 || !player.isGrounded)
        {
            timeFromLastStep = 0;
        }
        if (!player.isGrounded && Time.time > 0.1f)
        {
            playLanding = true;
        }
        if (player.isGrounded)
        {
            if (playLanding)
            {
                source.resource = landing;
                source.Play();
                playLanding = false;
            }
        }
    }
}
