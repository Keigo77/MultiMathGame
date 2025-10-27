using Fusion;
using TMPro;
using UnityEngine;

public class PlayerController : NetworkBehaviour
{
    [SerializeField] private float _moveSpeed;
    [SerializeField] private float _jumpForce;
    [SerializeField] private TextMeshPro _playerNameText;
    private Rigidbody2D _rigidbody;
    private bool _isTouchingGround = false;

    public override void Spawned()
    {
        _rigidbody = this.GetComponent<Rigidbody2D>();
        // プレイヤー名の更新
        _playerNameText.text = PlayerInfoManager.PlayerName;
    }
    
    public override void Render()
    {
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

        // 地面についている間だけジャンプ可能．
        if (Input.GetKeyDown(KeyCode.Space) && _isTouchingGround)
        {
            _isTouchingGround = false;
            _rigidbody.AddForce(Vector2.up * _jumpForce, ForceMode2D.Impulse);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            _isTouchingGround = true;
        }
    }
}
