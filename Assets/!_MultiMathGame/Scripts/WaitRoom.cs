using System;
using System.Collections.Generic;
using Fusion;
using Fusion.Sockets;
using UnityEngine;
using UnityEngine.UI;

public class WaitRoom : NetworkBehaviour, INetworkRunnerCallbacks
{
    public static int JoinedPlayer = 1;
    [SerializeField] private NetworkPrefabRef _playerPrefab;
    [SerializeField] private Color[] _playerColors;
    [SerializeField] private Button _startButton;

    // GameLauncherでSceneを指定することで，自動でこのオブジェクト(NetworkObject)がスポーンする．
    public override void Spawned()
    {
        // NetworkBehaviourを継承していれば，RunnerでNetworkRunnerにアクセス可能．
        // ここでrunner = FindFirstObjectByType<NetworkRunner>();とすると，NullReferenceエラーが出る．
        Runner.AddCallbacks(this);

        // ホストなら，スタートボタンを表示する．
        if (Runner.IsSharedModeMasterClient)
        {
            _startButton.gameObject.SetActive(true);
        }
    }
    
    public void StartButtonOnClick()
    {
        // IsOpenをfalseにしてからMainに行かないと，プレイ中にMainシーンに人が入って来れるようになる．
        Runner.SessionInfo.IsOpen = false;
        JoinedPlayer = Runner.SessionInfo.PlayerCount;
        Runner.LoadScene("Main");
        // SceneManager.LoadScene("Main");だと押した人しかシーン遷移しない．
    }

    private void OnDisable()
    {
        // OnDestroyのタイミングでRemoveCallbacksすると，MainでPlayerOnLeftが呼ばれなくなる．
        // StartButtonOnClickでRemoveCallbacksすると，呼ばれてしまう．=>ホストしかコールバックを解除していないから．
        if (Runner != null)
        {
            Runner.RemoveCallbacks(this);
        }
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
                
                // プレイヤーの色の更新．現在部屋にいるプレイヤーの数を取得し，入ってきた順番で色を決定する．
                int nowPlayerCount = runner.SessionInfo.PlayerCount - 1;
                playerController.PlayerColor = _playerColors[nowPlayerCount];
                
                // Mainシーンでも同じ色を使いたいため，色を保存する．
                PlayerInfoManager.PlayerColor = _playerColors[nowPlayerCount];
            });
        }

        // ホストかつ，部屋に2人以上いるなら，スタートボタンを押せるようにする．
        if (runner.IsSharedModeMasterClient && runner.SessionInfo.PlayerCount >= 1)
        {
            _startButton.interactable = true;
        }
    }

    // ここでrunnerでなく，Runnerを使うと，Runnerはnullなのでエラーが出る
    void INetworkRunnerCallbacks.OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        Debug.Log(player);
        if (runner.IsSharedModeMasterClient && runner.SessionInfo.PlayerCount < 2)
        {
            _startButton.interactable = false;
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
