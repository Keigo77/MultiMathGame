using Fusion;
using UnityEngine;

public class PlayerSpawner : NetworkBehaviour
{
    [SerializeField] private NetworkPrefabRef _playerPrefab;
    [SerializeField] private GameManager _gameManager;
    
    public override void Spawned()
    {
        var playerAvatar = Runner.Spawn(_playerPrefab, onBeforeSpawned: (_, playerObj) =>
        {
            PlayerController playerController = playerObj.GetComponent<PlayerController>();
            playerController.PlayerName = PlayerInfoManager.PlayerName;
            playerController.PlayerColor = PlayerInfoManager.PlayerColor;
            playerController.Init(_gameManager);
        });
            
        // 自分自身のプレイヤーオブジェクトを設定する．(他のプレイヤーが，簡単に他Playerを取得できるようになる)
        Runner.SetPlayerObject(Runner.LocalPlayer, playerAvatar);
        PlayerCountCheck();
    }

    private void PlayerCountCheck()
    {
        if (WaitRoom.JoinedPlayer == Runner.SessionInfo.PlayerCount)
        {
            _gameManager.RpcStartGame();
        }
    }
}
