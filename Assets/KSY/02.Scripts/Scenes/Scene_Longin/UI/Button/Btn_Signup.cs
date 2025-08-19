using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Btn_Signup : MonoBehaviour
{
    [SerializeField] private TMP_InputField input_ID;
    [SerializeField] private TMP_InputField input_Pw;
    [SerializeField] private TMP_InputField input_Nickname;
    public void Signup()
    {
        string id = input_ID.text;
        string pw = input_Pw.text;
        string nickname = input_Nickname.text;

        if(string.IsNullOrWhiteSpace(nickname))
        {
            Debug.Log("닉네임이나 아이디를 모두 입력해주세요.");
            UIManager.Instance.UpdateText("Login/Text_ErrorInfo", "121212");
            return;
        }

        BackendManager.Instance.Signup(id, pw);
        BackendManager.Instance.UpdateNickname(nickname);
    }
}
