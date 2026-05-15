using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Managment
{
    public class GameInputManager : MonoBehaviour
    {
        [Header("Input Actions")]
        [Tooltip("Drag here your pause InputAction from the input Action window")]
        [SerializeField] private InputActionReference pauseActionReference;

        private void OnEnable()
        {
            if (!pauseActionReference)
            {
                Debug.LogWarning("GameInputManager is missing a reference to the Pause Action!");
                return;
            }
            
            pauseActionReference.action.Enable();
            pauseActionReference.action.performed += OnPausePreformed;
        }

        private void OnDisable()
        {
            if (pauseActionReference)
            {
                pauseActionReference.action.Disable();
                pauseActionReference.action.performed -= OnPausePreformed;
            }
        }

        private void OnPausePreformed(InputAction.CallbackContext context)
        {
            if(GameManager.Instance.CurrentState == GameManager.GameState.Gameplay) EventManager.RaisePauseGame();
            else if (GameManager.Instance.CurrentState == GameManager.GameState.Paused) EventManager.RaiseResumeGame();
        }
    }
}