using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Fusion;
using Fusion.Sockets;
using TMPro;
using UnityEngine;

public class GameManager : NetworkBehaviour, INetworkRunnerCallbacks
{
    [Networked] public NetworkBool IsGameStarted { get; private set; } = false;
    [SerializeField] private TextMeshProUGUI _resultText;
    private List<PlayerRef> _deathPlayers = new List<PlayerRef>();

    // UniTask
    private CancellationToken _token;

    public override void Spawned()
    {
        Runner.AddCallbacks(this);
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

    private void Disbled()
    {
        Runner.RemoveCallbacks(this);
    }

    // -----------------INetworkRunnerCallbacks-------------------------

    void INetworkRunnerCallbacks.OnPlayerJoined(NetworkRunner runner, PlayerRef player) {}

    void INetworkRunnerCallbacks.OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        if (runner.SessionInfo.PlayerCount == 1)
        {
            _resultText.text = "You Win!";
            _resultText.gameObject.SetActive(true);
            MoveWaitRoom().Forget();
        }
    }

    void INetworkRunnerCallbacks.OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) {}
    void INetworkRunnerCallbacks.OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) {}
    void INetworkRunnerCallbacks.OnInput(NetworkRunner runner, NetworkInput input) {}
    void INetworkRunnerCallbacks.OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) {}
    void INetworkRunnerCallbacks.OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason) {}
    void INetworkRunnerCallbacks.OnConnectedToServer(NetworkRunner runner) {}
    void INetworkRunnerCallbacks.OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason) {}
    void INetworkRunnerCallbacks.OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) {}
    void INetworkRunnerCallbacks.OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) {}
    void INetworkRunnerCallbacks.OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) {}
    void INetworkRunnerCallbacks.OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList) {}
    void INetworkRunnerCallbacks.OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) {}
    void INetworkRunnerCallbacks.OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) {}
    void INetworkRunnerCallbacks.OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data) {}
    void INetworkRunnerCallbacks.OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress) {}
    void INetworkRunnerCallbacks.OnSceneLoadDone(NetworkRunner runner) {}
    void INetworkRunnerCallbacks.OnSceneLoadStart(NetworkRunner runner) {}
}
