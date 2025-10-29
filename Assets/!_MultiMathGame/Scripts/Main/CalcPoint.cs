using Fusion;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CalcPoint : NetworkBehaviour
{
    [SerializeField] private GameObject _calcPanelObj;
    [SerializeField] private TextMeshProUGUI _questionText;
    [SerializeField] private TMP_InputField _answerInputField;
    
    [Networked] private NetworkBool IsAnyPlayerTouched { get; set; }= false;
    private SpriteRenderer _spriteRenderer;
    private int _answer;
    private PlayerController _playerController;
    private NetworkObject _networkObject;
    private bool _isPushedDecideButton = false;
    
    public override void Spawned()
    {
        _spriteRenderer = this.GetComponent<SpriteRenderer>();
        _networkObject = this.GetComponent<NetworkObject>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !IsAnyPlayerTouched && collision.gameObject.GetComponent<NetworkObject>().HasStateAuthority)
        {
            Debug.Log("問題");
            
            _spriteRenderer.enabled = false;
            // プレイヤーが動けないようにする．既に問題を解いている最中なら，何もしない．
            _playerController = collision.GetComponent<PlayerController>();
            if (_playerController.IsShowingQuestion)
            {
                Runner.Despawn(_networkObject);
                return;
            }
            
            _playerController.IsShowingQuestion = true;
            IsAnyPlayerTouched = true;
            
            // 問題作成
            var question = new CreateQuestion();
            _questionText.text = $"{question.num1} + {question.num2} = ?";
            _answer = question.result;
            
            _calcPanelObj.SetActive(true);
        }
    }

    public void DecideButtonOnClick()
    {
        if (_isPushedDecideButton) { return; }
        _isPushedDecideButton = true;
        
        if (_playerController != null)
        {
            _playerController.IsShowingQuestion = false;
        }
        
        if (int.TryParse(_answerInputField.text, out int playerAnswer) && playerAnswer == _answer)
        {
            Correct();
            Debug.Log("正解");
        }
        else
        {
            InCorrect();
            Debug.Log("不正解");
        }
    }

    /// <summary>
    /// 自分以外のプレイヤーにダメージ
    /// </summary>
    private void Correct()
    {
        foreach (var player in Runner.ActivePlayers) {
            if (Runner.TryGetPlayerObject(player, out var playerObj) && player != Runner.LocalPlayer) {
                if (playerObj.TryGetComponent<PlayerController>(out var playerController))
                {
                    playerController.RpcDamage();
                }
            }
        }
        
        RpcDespawn();
    }

    /// <summary>
    /// 自分のみにダメージ
    /// </summary>
    private void InCorrect()
    {
        foreach (var player in Runner.ActivePlayers) {
            if (Runner.TryGetPlayerObject(player, out var playerObj) && player == Runner.LocalPlayer) {
                if (playerObj.TryGetComponent<PlayerController>(out var playerController))
                {
                    playerController.RpcDamage();
                }
            }
        }
        
        RpcDespawn();
    }

    /// <summary>
    /// ホストがスポーンさせている物なので，ホストしかデスポーンさせられない．
    /// </summary>
    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    private void RpcDespawn()
    {
        Runner.Despawn(_networkObject);
    }
}
