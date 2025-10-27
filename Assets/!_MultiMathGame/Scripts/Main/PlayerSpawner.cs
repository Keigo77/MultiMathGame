using System;
using System.Collections.Generic;
using Fusion;
using Fusion.Sockets;
using UnityEngine;

public class PlayerSpawner : NetworkBehaviour, INetworkRunnerCallbacks
{
    [SerializeField] private NetworkPrefabRef _playerPrefab;
    [SerializeField] private GameManager _gameManager;
    
    public override void Spawned()
    {
        Runner.AddCallbacks(this);
    }
    
    
    
    // -----------------INetworkRunnerCallbacks-------------------------
    
    void INetworkRunnerCallbacks.OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        // OnPlayerJoinedはプレイヤーが入室する度，全プレイヤーで実行されるため，自分自身だけがアバターをスポーンさせないといけない．
        if (player == runner.LocalPlayer)
        {
            runner.Spawn(_playerPrefab, onBeforeSpawned: (_, playerObj) =>
            {
                PlayerController playerController = playerObj.GetComponent<PlayerController>();
                playerController.PlayerName = PlayerInfoManager.PlayerName;
                playerController.PlayerColor = PlayerInfoManager.PlayerColor;
            });
        }

        // スタートボタンを押したときのプレイヤー全員のシーン遷移が終わったら，ゲーム開始！
        if (Runner.SessionInfo.PlayerCount == WaitRoom.JoinedPlayer)
        {
            Debug.Log("ゲーム開始！！");
            _gameManager._gameState = GameManager.GameState.Started;
        }
    }

    void INetworkRunnerCallbacks.OnPlayerLeft(NetworkRunner runner, PlayerRef player) { }
    
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
