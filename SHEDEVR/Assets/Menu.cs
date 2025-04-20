using System.IO;
using UnityEditor.Overlays;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    private string savePath;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        savePath = Path.Combine(Application.persistentDataPath, "playerSave.txt");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void onStartNewGame()
    {
        File.WriteAllText(savePath, "//////");
        SceneManager.LoadScene("Gameplay Scene");

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
