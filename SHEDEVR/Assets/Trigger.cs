using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Trigger : MonoBehaviour
{
    [SerializeField] DoorScript[] doorsBack;
    [SerializeField] DoorScript[] doorsNext;
    [SerializeField] GameObject location;
    [SerializeField] GameObject save;
    [SerializeField] GameObject nextLocation;
    private GameObject _nextLocation;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _nextLocation = null;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        SceneManager.LoadScene(2);
    }
    IEnumerator destroyLocation(GameObject location)
    {
        if (location != null)
        {
            yield return new WaitForSeconds(2f);
            save.transform.parent = null;
            foreach (Transform t in location.transform)
            {
                if (t.gameObject.layer != 10) Destroy(t.gameObject);
            }
        }
    }
}
