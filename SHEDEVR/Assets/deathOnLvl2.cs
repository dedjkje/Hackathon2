using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class deathOnLvl2 : MonoBehaviour
{
    [SerializeField] FirstPersonController firstPersonController;
    bool canPlay = true;
    AudioSource source;
    [SerializeField] int sceneId;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        source = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if ((firstPersonController == null || firstPersonController.isDead) && canPlay)
        { 
            canPlay = false;
            source.Play();
            StartCoroutine(death());
        }
    }
    IEnumerator death()
    {
        yield return new WaitForSecondsRealtime(3.5f);
        SceneManager.LoadScene(sceneId);
    }
}
