using BackEnd;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BtnSignupListener : MonoBehaviour
{
    [SerializeField] private TMP_InputField input_ID;
    [SerializeField] private TMP_InputField input_Pw;
    [SerializeField] private TMP_InputField input_Nickname;
    public void Signup()
    {
        string id = input_ID.text;
        string pw = input_Pw.text;
        string nickname = input_Nickname.text;

        //�Է¹��� �г����� �� ĭ(���� ����)�� ��� ó��
        if(string.IsNullOrWhiteSpace(nickname))
        {
            Debug.Log("�г����̳� ���̵� ��� �Է����ּ���.");
            UIManager.Instance.UpdateText("Login/Text_ErrorInfo", "121212");
            return;
        }

        //ȸ�� ���� �õ�
        int signup_statusCode = ServerManager.Instance.TrySignup(id, pw);

        switch(signup_statusCode)
        {
            //ȸ�� ���� ���� ó��
            case 201:
                {
                    UIManager.Instance.UpdateText("Login/Text_ErrorInfo", "Registration successful");
                    break;
                }
            //��ĭ ����ó��
            case 400:
                {
                    //text : ���̵�� ��й�ȣ�� ��� ĭ�� �����ּ���.
                    UIManager.Instance.UpdateText("Login/Text_ErrorInfo", "22222");
                    break;
                }
            //���̵� �ߺ� ó��
            case 409:
                {
                    UIManager.Instance.UpdateText("Login/Text_ErrorInfo", "11111");
                    break;
                }
            //�� �ۿ� ����ó��
            default:
                {
                    //text : Error ! �ٽ� �õ����ּ���.
                    UIManager.Instance.UpdateText("Login/Text_ErrorInfo", "33333");
                    break;
                }
        }

        //�г��� ���� �õ�
        int updateNicknameStatuscode = ServerManager.Instance.TryUpdateNickName(nickname);

        switch (updateNicknameStatuscode)
        {
            //�г��� ���� ���� ó��
            case 204:
                {
                    UIManager.Instance.UpdateText("Login/Text_ErrorInfo", "Registration successful");
                    break;
                }
            //�г����� 20�� �̻��� ��� ó��
            case 400:
                {
                    Debug.LogError("�г����� �ʹ� ��ϴ�. �ٸ� �г����� �������ּ���.");
                    break;
                }
            //�̹� �ߺ��� �г����� �ִ� ��� ó��
            case 409:
                {
                    Debug.LogError("�ߺ��� �г����Դϴ�. �ٸ� �г����� �������ּ���.");
                    break;
                }
            //�� �ۿ� ����ó��
            default:
                {
                    //text : Error ! �ٽ� �õ����ּ���.
                    UIManager.Instance.UpdateText("Login/Text_ErrorInfo", "33333");
                    break;
                }
        }
    }
}
