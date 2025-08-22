using UnityEngine;
using UnityEngine.SceneManagement;

public class ToMain : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Cursor.lockState = CursorLockMode.None;
    }

    // Update is called once per frame
    public void MainMenu()
    {
        SceneManager.LoadScene("TitleScreen");
    }
}
