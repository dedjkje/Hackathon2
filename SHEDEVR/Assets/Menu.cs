using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void onStartNewGame()
    {
        SceneManager.LoadScene(1);

    }
    public void onContinueGame()
    {
        SceneManager.LoadScene("Gameplay Scene");
    }

    public void onExitButton()
    {
        Application.Quit();
    }
}
