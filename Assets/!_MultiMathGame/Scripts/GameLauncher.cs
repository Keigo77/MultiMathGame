using Fusion;
using TMPro;
using UnityEngine;

public class GameLauncher : MonoBehaviour
{
    [SerializeField] private NetworkRunner _networkRunnerPrefab;
    [SerializeField] private TMP_InputField _roomNameInputField;
    
    public async void RandomMatchButtonOnClick()
    {
        var networkRunner = Instantiate(_networkRunnerPrefab);
        
        // 共有モードのセッションに参加する
        var result = await networkRunner.StartGame(new StartGameArgs {
            GameMode = GameMode.Shared,
            PlayerCount = 4,
        });
        
        // 結果をコンソールに出力する
        Debug.Log(result);
    }
    
    public async void PrivateMatchButtonOnClick()
    {
        var networkRunner = Instantiate(_networkRunnerPrefab);
        
        // 共有モードのセッションに参加する．同じパスワードを入力した人同士でしかマッチングしない．
        var result = await networkRunner.StartGame(new StartGameArgs {
            GameMode = GameMode.Shared,
            SessionName = _roomNameInputField.text,
            PlayerCount = 4,
            IsVisible = false
        });
        
        // 結果をコンソールに出力する
        Debug.Log(result);
    }
}
