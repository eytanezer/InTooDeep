using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Managment
{
    public class GameManager : MonoSingleton<GameManager>
    {
        
        public enum GameState
        {
            Title,
            Instructions,
            Gameplay,
            GameOver
        }
   
        

        private void Awake()
        {
            
        }
        
        private void Start()
        {
            
        }

        private void OnEnable()
        {
            Cheats.OnResetGame += ResetGame;
            Cheats.OnQuit += QuitGame;
       
        }
    
        private void OnDisable()
        {
            Cheats.OnResetGame -= ResetGame;
            Cheats.OnQuit -= QuitGame;
        }
        
    
        public void QuitGame()
        {
#if UNITY_EDITOR
            // Application.Quit() does not work in the editor
            // so we use this instead
            UnityEditor.EditorApplication.isPlaying = false;
#else
        // Close the game!
        Application.Quit();
#endif
        }

  


        private void ResetGame()
        {
            // Scene currentScene = SceneManager.GetActiveScene();
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
        
    }
}
