using System;
using System.Collections;
using System.Collections.Generic;
using TastyWorld.Levels;
using UnityEngine;
using Zenject;

public class GameController : IInitializable, ITickable, IDisposable
{
    public GameStates CurrentGameState { private set; get; }
    private LevelManager _levelManager;

    public GameController(LevelManager levelManager)
    {
        _levelManager = levelManager;
    }
    
    public void Initialize()
    {
        CurrentGameState = GameStates.Playing;
    }

    public void Tick()
    {
        switch (CurrentGameState)
        {
            case GameStates.Playing:
                _levelManager.UpdateLevelTimer(Time.deltaTime);
                break;
            case GameStates.LevelCompleted:
                break;
            case GameStates.LevelFailed:
                break;
            case GameStates.BoosterShop:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public void Dispose()
    {
        
    }

    public void RestartLevel()
    {
        if (CurrentGameState != GameStates.LevelFailed)
            return;
        ChangeGameState(GameStates.Playing);
        _levelManager.RestartLevel();
    }

    private void ChangeGameState(GameStates newState)
    {
        CurrentGameState = newState;
    }
    
    public enum GameStates
    {
        Playing,
        LevelCompleted,
        LevelFailed,
        BoosterShop,
        Count,
    }
}
