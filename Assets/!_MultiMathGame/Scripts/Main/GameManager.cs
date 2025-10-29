using System;
using System.Collections.Generic;
using System.Linq;
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
    private int DeathPlayer = 0;
    private List<PlayerRef> _deathPlayerRef = new List<PlayerRef>();
    

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RpcDeath(PlayerRef player)
    {
        _deathPlayerRef.Add(player);
        
        DeathPlayer++;
        if (DeathPlayer == Runner.SessionInfo.PlayerCount - 1)
        {
            State = GameState.Finished;
            CheckGameFinish();
        }
    }

    private void CheckGameFinish()
    {
        if (!_deathPlayerRef.Contains(Runner.LocalPlayer))
        {
            Debug.Log("You Win!");
        } else if (_deathPlayerRef.Contains(Runner.LocalPlayer))
        {
            Debug.Log("You Lose...");
        }
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RpcStartGame()
    {
        State = GameState.Started;
    }
}
