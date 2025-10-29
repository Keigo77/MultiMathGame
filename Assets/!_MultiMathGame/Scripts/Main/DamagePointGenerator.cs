using Fusion;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DamagePointGenerator : NetworkBehaviour
{
    [SerializeField] private NetworkPrefabRef _calcPointPrefab;
    [SerializeField] private float _generateSpan;
    [Networked] private int StartTick { get; set; } // 開始ティック
    [SerializeField] private GameManager _gameManager;
    
    public override void Spawned() {
        if (HasStateAuthority) {
            // スポーン時の現在ティックを、開始ティックとして設定する
            StartTick = Runner.Tick;
        }
    }

    public override void Render() {
        
        if (!HasStateAuthority) { return; }
        
        float elapsedTime = (Runner.Tick - StartTick) * Runner.DeltaTime;
        if (elapsedTime >= _generateSpan && _gameManager.IsGameStarted)
        {
            // 経過時間のリセット．elapsedTime=0としても，Render内では実行されない．
            StartTick = Runner.Tick;
            
            var randX = Random.Range(-8f, 8f);
            var randY = Random.Range(-3f, 4f);

            // NetworkTransformを付けて，Positionを共有する．
            Runner.Spawn(_calcPointPrefab, new Vector2(randX, randY));
        }
    }
}
