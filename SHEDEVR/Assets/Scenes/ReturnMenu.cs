using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ReturnMenu : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        back();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    IEnumerator back()
    {
        yield return new WaitForSeconds(5f);
        SceneManager.LoadScene(0);
    }
}
