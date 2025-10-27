using UnityEngine;

public class PlayerInfoManager : MonoBehaviour
{
    public static string PlayerName { get; private set; }

    /// <summary>
    /// タイトルシーンのユーザーネーム入力欄の中身を変更するごとに，ユーザーネームを更新．
    /// </summary>
    public void UserNameInputFieldOnValueChanged(string value)
    {
        PlayerName = value;
    }
}
