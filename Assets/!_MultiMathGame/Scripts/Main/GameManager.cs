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
    
    [Networked] public GameState State { get; set; } = GameState.None;

    public void FixedUpdateNetwork()
    {
        if (State != GameState.Started) { return; }
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RpcStartGame()
    {
        State = GameState.Started;
    }
}
