# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

A 2D Unity game (Unity 6000.3.11f1) with swimming mechanics, using URP 2D rendering and the new Input System.

## Unity Development

Open the project in **Unity 6000.3.11f1**. There is no CLI build command — use the Unity Editor or Unity Hub.

- **Run tests:** Edit > Test Runner (uses `com.unity.test-framework`)
- **Enter Play Mode:** Press the Play button in the Unity Editor (or Ctrl+P)
- **Build:** File > Build Settings

## Architecture

### Singleton Management (`Assets/Scripts/Managment/`)

`MonoSingleton<T>` is a generic base class for persistent singletons (survives scene loads via `DontDestroyOnLoad`). `GameManager` extends it, owns game state (`Title → Instructions → Gameplay → GameOver`), and handles scene reload and quit.

### Event-Driven Cheat System (`Cheats.cs`)

`Cheats` raises static C# events using Unity's Input System (Shift+1/2/3/Q). Other systems subscribe to these events rather than polling — `PlayerMovement` and `GameManager` both subscribe to respond to resets.

### Player (`Assets/Scripts/Player/PlayerMovement.cs`)

Force-based Rigidbody2D movement. Input comes from the new Input System via `OnMove(InputValue)` callback. Speed is capped each `FixedUpdate` by clamping velocity magnitude to `maxSpeed`. Stores spawn position on `Start` for cheat-triggered resets.

### Enemies (`Assets/Scripts/Enemies/`)

Folder exists but is empty — intended for future enemy AI implementation.

## Key Packages

| Package | Version | Purpose |
|---|---|---|
| Universal RP | 17.3.0 | 2D URP rendering |
| Input System | 1.19.0 | Player & cheat input |
| Cinemachine | 3.1.6 | Camera control |
| 2D Tilemap | 1.0.0 | Level layout |
| com.unity.2d.aseprite | 3.0.1 | Sprite import |

## Scenes

- `Assets/Scenes/SampleScene.unity` — main scene
- `Assets/Scenes/Scene1.unity` — secondary scene
- `Assets/Settings/Scenes/URP2DSceneTemplate.unity` — URP template (do not edit)
