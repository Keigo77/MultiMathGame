using Fusion;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class PlayerController : NetworkBehaviour
{
    private int _playerHp = 10;
    private int _playerMaxHp;
    [SerializeField] private Image _greenHpGauge;
    [Networked] public NetworkString<_8> PlayerName { get; set; }
    [Networked] public Color PlayerColor { get; set; }
    [SerializeField] private float _moveSpeed;
    [SerializeField] private float _jumpForce;
    [SerializeField] private TextMeshPro _playerNameText;
    private Rigidbody2D _rigidbody;
    private NetworkObject _networkObject;
    private GameManager _gameManager;
    
    private const float CASTRADIUS = 0.2f;
    private const float CASTDISTANCE = 0.5f;


    public void Init(GameManager gameManager)
    {
        _gameManager = gameManager;
    }

    public override void Spawned()
    {
        _networkObject = this.GetComponent<NetworkObject>();
        _rigidbody = this.GetComponent<Rigidbody2D>();
        _playerMaxHp = _playerHp;
        
        // プレイヤー名・色の更新
        _playerNameText.text = PlayerName.Value;
        this.GetComponent<SpriteRenderer>().color = PlayerColor;
    }
    
    // FixedUpdateNetworkだと，スペースキーが反応しないことがある．
    public override void Render()
    {
        // 地面についている間だけジャンプ可能．
        bool isTouchingGround = Physics2D.CircleCast(transform.position, CASTRADIUS, Vector2.down, CASTDISTANCE, LayerMask.GetMask("Ground"));
        
        if (Input.GetKeyDown(KeyCode.Space) && isTouchingGround)
        {
            _rigidbody.AddForce(Vector2.up * _jumpForce, ForceMode2D.Impulse);
        }
        
        if (Input.GetKey(KeyCode.A))
        {
            _rigidbody.linearVelocityX = -_moveSpeed;
        } 
        else if (Input.GetKey(KeyCode.D))
        {
            _rigidbody.linearVelocityX = _moveSpeed;
        }
        else
        {
            _rigidbody.linearVelocityX = 0f;
        }
    }

    [Rpc(RpcSources.Proxies, RpcTargets.All)]
    public void RpcDamage()
    {
        _playerHp -= 4;
        _greenHpGauge.fillAmount = _playerHp / (float)_playerMaxHp;

        if (_playerHp <= 0)
        {
            _gameManager.RpcSendDeath(Runner.LocalPlayer);
            Runner.Despawn(_networkObject);
        }
    }
    
    /// <summary>
    /// Unityエディター上に，CircleCastの範囲を表示する
    /// </summary>
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Vector3 origin = transform.position;
        Vector3 endPoint = origin + (Vector3)Vector2.down * CASTDISTANCE;
        Gizmos.DrawWireSphere(endPoint, CASTRADIUS);
    }
}
