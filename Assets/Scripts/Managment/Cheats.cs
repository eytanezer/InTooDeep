using UnityEngine;
using System;
using UnityEngine.InputSystem;
public class Cheats : MonoBehaviour
{
    public static event Action OnResetGame;
    public static event Action OnResetPlayersPosition;
    public static event Action OnResetVehiclesSpawning;
    public static event Action OnQuit;

    void Update()
    {
        if (Keyboard.current == null) return;
        bool isModifierHeld = Keyboard.current.leftShiftKey.isPressed || Keyboard.current.rightShiftKey.isPressed;
        if (isModifierHeld && Keyboard.current.digit1Key.wasPressedThisFrame) // reset game
        {
            OnResetGame?.Invoke();
            print("Game Reset");
        }
        if (isModifierHeld && Keyboard.current.digit2Key.wasPressedThisFrame) // reset player position
        {
            OnResetPlayersPosition?.Invoke();
            print("Players Position Reset");
        }
        if (isModifierHeld && Keyboard.current.digit3Key.wasPressedThisFrame) // reset enemy positions
        {
            OnResetVehiclesSpawning?.Invoke();
            print("Vehicles Spawning Reset");
        }
        
        if (isModifierHeld && Keyboard.current.qKey.wasPressedThisFrame) // reset health
        {
            OnQuit?.Invoke();
            print("quit");
        }
        
    }
}
