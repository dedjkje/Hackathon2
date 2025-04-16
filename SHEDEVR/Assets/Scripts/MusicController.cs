using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class MusicController : MonoBehaviour
{
    [SerializeField] private AudioClip musicClip;
    [SerializeField] private float muteDuration = 1f; // Длительность заглушения

    private AudioSource audioSource;
    private float originalVolume;
    private Coroutine muteCoroutine;
    public Settings settings;
    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = musicClip;
        audioSource.loop = true;
        originalVolume = settings.volume * 0.5f;
    }

    private void Start()
    {
        audioSource.Play();
    }

    public void TemporaryMute(float duration)
    {
        if (muteCoroutine != null)
            StopCoroutine(muteCoroutine);

        muteCoroutine = StartCoroutine(MuteRoutine(duration));
    }

    private IEnumerator MuteRoutine(float muteTime)
    {

        audioSource.volume = 0f;

        yield return new WaitForSeconds(muteTime + 2f);


        float timer = 0f;
        float fadeInDuration = 1f;

        while (timer < fadeInDuration)
        {
            timer += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(0f, originalVolume, timer / fadeInDuration);
            yield return null;
        }

        audioSource.volume = originalVolume;
    }
}