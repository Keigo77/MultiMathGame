using Fusion;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DamagePoint : NetworkBehaviour
{
    private PlayerController _playerController;
    private NetworkObject _networkObject;
    
    public override void Spawned()
    {
        _networkObject = this.GetComponent<NetworkObject>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && collision.gameObject.GetComponent<NetworkObject>().HasStateAuthority 
                                           && TryGetComponent<PlayerController>(out var playerController))
        {
            foreach (var player in Runner.ActivePlayers) {
                if (player != Runner.LocalPlayer) {
                    playerController.RpcDamage();
                }
            }
        }
        
        RpcDespawn();
    }

    /// <summary>
    /// このオブジェクトを生成したホストしかでスポーンさせることができないので，デスポーンをホストに依頼する．
    /// </summary>
    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    private void RpcDespawn()
    {
        Runner.Despawn(_networkObject);
    }
}
