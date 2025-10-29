using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Fusion;
using TMPro;
using UnityEngine;

public class GameManager : NetworkBehaviour
{
    [Networked] public NetworkBool IsGameStarted { get; private set; } = false;
    [SerializeField] private TextMeshProUGUI _resultText;
    private List<PlayerRef> _deathPlayers = new List<PlayerRef>();

    // UniTask
    private CancellationToken _token;

    public override void Spawned()
    {
        _token = this.GetCancellationTokenOnDestroy();
    }

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
            if (_deathPlayers.Contains(Runner.LocalPlayer))
            {
                _resultText.text = "You Lose...";
            }
            else
            {
                _resultText.text = "You Win!";
            }
            
            _resultText.gameObject.SetActive(true);
            MoveWaitRoom().Forget();
        }
    }

    private async UniTaskVoid MoveWaitRoom()
    {
        if (HasStateAuthority)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(1.5f), cancellationToken: _token);
            Runner.LoadScene("WaitRoom");
        }
    }
}
