using UnityEngine;
using UnityEngine.SceneManagement;
using Managment;

public class RestartController : MonoBehaviour
{
    private void OnEnable()
    {
        EventManager.OnResetGame += RestartCurrentScene;
        Cheats.OnResetGame += RestartCurrentScene;
    }

    private void OnDisable()
    {
        EventManager.OnResetGame -= RestartCurrentScene;
        Cheats.OnResetGame -= RestartCurrentScene;
    }

    private void RestartCurrentScene()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}