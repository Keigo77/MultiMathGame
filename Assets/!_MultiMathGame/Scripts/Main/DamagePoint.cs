using Fusion;
using UnityEngine;

public class DamagePoint : NetworkBehaviour
{
    private NetworkObject _networkObject;
    
    public override void Spawned()
    {
        _networkObject = this.GetComponent<NetworkObject>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // デスポーンまでに遅延があり，2回触れることもできるため，誰かが触れたら即座に非表示にする．
            this.gameObject.SetActive(false);
            
            if (collision.gameObject.GetComponent<NetworkObject>().HasStateAuthority)
            {
                foreach (var player in Runner.ActivePlayers) {
                    if (Runner.TryGetPlayerObject(player, out var playerObj) && player != Runner.LocalPlayer)
                    {
                        playerObj.GetComponent<PlayerController>().RpcDamage();
                    }
                }
            
                RpcDespawn();
            }
        }
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
