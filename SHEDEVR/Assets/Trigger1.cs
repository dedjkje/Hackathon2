using System.Collections;
using UnityEditor;
using UnityEngine;

public class Trigger1 : MonoBehaviour
{
    [SerializeField] DoorScript[] doorsBack;
    [SerializeField] DoorScript[] doorsNext;
    [SerializeField] GameObject location;
    [SerializeField] GameObject save;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        foreach (DoorScript door in doorsBack)
        {
            door.Transaction();
        }
        foreach (DoorScript door in doorsNext)
        {
            door.BrainSwap();
        }
        StartCoroutine(destroyLocation(location));
    }

    // Update is called once per frame
    void Update()
    {

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
