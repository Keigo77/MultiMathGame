using Fusion;
using TMPro;
using UnityEngine;

public class PlayerController : NetworkBehaviour
{
    [Networked] public NetworkString<_8> PlayerName { get; set; }
    [Networked] public Color PlayerColor { get; set; }
    
    [SerializeField] private float _moveSpeed;
    [SerializeField] private float _jumpForce;
    [SerializeField] private TextMeshPro _playerNameText;
    private Rigidbody2D _rigidbody;
    
    private const float _castRadius = 0.2f;
    private const float _castDistance = 0.5f;

    public override void Spawned()
    {
        _rigidbody = this.GetComponent<Rigidbody2D>();
        
        // プレイヤー名・色の更新
        _playerNameText.text = PlayerName.Value;
        this.GetComponent<SpriteRenderer>().color = PlayerColor;
    }
    
    // FixedUpdateNetworkだと，スペースキーが反応しないことがある．
    public override void Render()
    {
        bool isTouchingGround = Physics2D.CircleCast(transform.position, _castRadius, Vector2.down, _castDistance, LayerMask.GetMask("Ground"));
        
        // 地面についている間だけジャンプ可能．
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
        if (Runner == null)
        {
            Debug.Log("Runner is null");
            return;
        }
    }
    
    
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Vector3 origin = transform.position;
        Vector3 endPoint = origin + (Vector3)Vector2.down * _castDistance;
        Gizmos.DrawWireSphere(endPoint, _castRadius);
    }
}
