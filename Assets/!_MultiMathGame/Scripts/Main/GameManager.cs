using System;
using System.Collections.Generic;
using Fusion;
using TMPro;
using UnityEngine;

public class GameManager : NetworkBehaviour
{
    [Networked] public NetworkBool IsGameStarted { get; private set; } = false;
    [SerializeField] private TextMeshProUGUI _resultText;
    private List<PlayerRef> _deathPlayers; 
    

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RpcStartGame()
    {
        IsGameStarted = true;
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RpcSendDeath(PlayerRef deathPlayer)
    {
        _deathPlayers.Add(deathPlayer);

        if (_deathPlayers.Count == Runner.SessionInfo.PlayerCount - 1)
        {
            IsGameStarted = false;

            if (_deathPlayers.Contains(Runner.LocalPlayer))
            {
                _resultText.text = "You Lose...";
            }
            else
            {
                _resultText.text = "You Win!";
            }
            
            _resultText.gameObject.SetActive(true);
        }
    }
}
