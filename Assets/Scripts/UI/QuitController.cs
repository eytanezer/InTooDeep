using UnityEngine;
using Managment;

public class QuitController : MonoBehaviour
{
    private void OnEnable()
    {
        EventManager.OnQuitGame += QuitGame;
        Cheats.OnQuit += QuitGame;
    }

    private void OnDisable()
    {
        EventManager.OnQuitGame -= QuitGame;
        Cheats.OnQuit -= QuitGame;
    }

    private void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}