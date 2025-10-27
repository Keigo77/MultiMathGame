using System;
using Fusion;
using UnityEngine;

public class GameManager : NetworkBehaviour
{
    public enum GameState
    {
        None,
        Started,
        Finished,
    }
    
    public GameState _gameState = GameState.None;

    public void FixedUpdateNetwork()
    {
        if (_gameState != GameState.Started) { return; }
    }
}
