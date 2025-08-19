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
            Debug.Log("�г����̳� ���̵� ��� �Է����ּ���.");
            UIManager.Instance.UpdateText("Login/Text_ErrorInfo", "121212");
            return;
        }

        BackendManager.Instance.Signup(id, pw);
        BackendManager.Instance.UpdateNickname(nickname);
    }
}
