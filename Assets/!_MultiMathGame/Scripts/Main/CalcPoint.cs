using Fusion;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CalcPoint : NetworkBehaviour
{
    // Generatorから受け取るもの
    [SerializeField] private GameObject _calcPanelObj;
    [SerializeField] private TextMeshProUGUI _questionText;
    [SerializeField] private TMP_InputField _answerInputField;
    
    [Networked] private NetworkBool IsPlayerTouched { get; set; }= false;
    private CircleCollider2D _circleCollider;
    private SpriteRenderer _spriteRenderer;
    private int answer;
    private PlayerController _playerController;
    private NetworkObject _networkObject;
    
    public override void Spawned()
    {
        _circleCollider = this.GetComponent<CircleCollider2D>();
        _spriteRenderer = this.GetComponent<SpriteRenderer>();
        _networkObject = this.GetComponent<NetworkObject>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !IsPlayerTouched)
        {
            // プレイヤーが動けないようにする．既に問題を解いている最中なら，何もしない．
            _playerController = collision.GetComponent<PlayerController>();
            if (_playerController.IsShowingQuestion)
            {
                Runner.Despawn(_networkObject);
                return;
            }
            else
            {
                _playerController.IsShowingQuestion = true;
            }
            
            IsPlayerTouched = true;
            
            // 問題作成
            var question = new CreateQuestion();
            _questionText.text = $"{question.num1} + {question.num2} = ?";
            answer = question.result;
            
            _calcPanelObj.SetActive(true);
        }
    }

    public void DecideButtonOnClick()
    {
        if (_playerController != null)
        {
            _playerController.IsShowingQuestion = false;
        }
        
        if (int.TryParse(_answerInputField.text, out int playerAnswer) && playerAnswer == answer)
        {
            Correct();
            Debug.Log("正解");
        }
        else
        {
            InCorrect();
            Debug.Log("不正解");
        }
        
        _calcPanelObj.SetActive(false);
    }

    /// <summary>
    /// 自分以外のプレイヤーにダメージ
    /// </summary>
    private void Correct()
    {
        foreach (var player in Runner.ActivePlayers) {
            if (Runner.TryGetPlayerObject(player, out var playerObj) && player != Runner.LocalPlayer) {
                var playerController = playerObj.GetComponent<PlayerController>();
                playerController.RpcDamage();
            }
        }
        
        Runner.Despawn(_networkObject);
    }

    /// <summary>
    /// 自分のみにダメージ
    /// </summary>
    private void InCorrect()
    {
        foreach (var player in Runner.ActivePlayers) {
            if (Runner.TryGetPlayerObject(player, out var playerObj) && player == Runner.LocalPlayer) {
                var playerController = playerObj.GetComponent<PlayerController>();
                playerController.RpcDamage();
            }
        }
        
        Runner.Despawn(_networkObject);
    }
}
