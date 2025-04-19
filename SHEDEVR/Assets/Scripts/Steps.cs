using UnityEngine;
using UnityEngine.Rendering;

public class Steps : MonoBehaviour
{
    [Header("Звуки")]
    [SerializeField] AudioClip[] steps;
    [SerializeField] AudioClip landing;

    [Header("Игрок")]
    [SerializeField] FirstPersonController player;

    [Header("Интервал шагов")]
    [SerializeField] float interval;

    private AudioSource source;
    private Vector2 currentSpeed;
    private Vector2 currentDelta;
    private float timeFromLastStep;
    private bool playLanding = false;
    private int nextStep = 0;

    void Start()
    {
        source = GetComponent<AudioSource>();
    }

    private void FixedUpdate()
    {
        currentSpeed = new Vector2(player.moveVector.x, player.moveVector.z);
        currentDelta = new Vector2(player.moveDelta.x, player.moveDelta.z);
    }

    void Update()
    {
        if (!player.changing) timeFromLastStep += Time.deltaTime * currentDelta.magnitude;

        if (timeFromLastStep > interval && player.isGrounded && !player.isDead && !player.changing)
        {
            source.resource = steps[nextStep];
            source.Play();
            timeFromLastStep = 0;
            nextStep = (nextStep + 1) % 5;
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
