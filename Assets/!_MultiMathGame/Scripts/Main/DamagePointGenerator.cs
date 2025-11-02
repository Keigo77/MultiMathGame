using Fusion;
using UnityEngine;

public class DamagePointGenerator : NetworkBehaviour
{
    [SerializeField] private NetworkPrefabRef _calcPointPrefab;
    [SerializeField] private float _generateSpan;
    private int _startTick;     // 開始時ティック
    [SerializeField] private GameManager _gameManager;
    
    public override void Spawned() {
        if (HasStateAuthority) {
            // スポーン時の現在ティックを、開始ティックとして設定する
            _startTick = Runner.Tick;
        }
    }

    public override void Render() {
        
        if (!HasStateAuthority) { return; }
        
        float elapsedTime = (Runner.Tick - _startTick) * Runner.DeltaTime;
        if (elapsedTime >= _generateSpan && _gameManager.IsGameStarted)
        {
            // 経過時間のリセット．elapsedTime=0としても，Render内では実行されない．
            _startTick = Runner.Tick;
            
            var randX = Random.Range(-8f, 8f);
            var randY = Random.Range(-3f, 4f);

            // NetworkTransformを付けて，Positionを共有する．
            Runner.Spawn(_calcPointPrefab, new Vector2(randX, randY));
        }
    }
}
