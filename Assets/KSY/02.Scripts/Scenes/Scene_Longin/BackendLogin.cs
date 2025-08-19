using System.Collections;
using BackEnd;
using BackEnd.Tcp;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BackendLogin
{
    private static BackendLogin _instance = null;
    public static BackendLogin Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new BackendLogin();
            }

            return _instance;
        }
    }

    public void CustomSignUp(string id, string pw)
    {
        var bro_customSignUp = Backend.BMember.CustomSignUp(id, pw);

        if (bro_customSignUp.IsSuccess()) //���������� ȸ������ �Ϸ�.
        {
            UIManager.Instance.UpdateText("Login/Text_ErrorInfo", "Registration successful");
        }
        else if (bro_customSignUp.StatusCode == 409) //���̵� �ߺ� üũ
        {
            UIManager.Instance.UpdateText("Login/Text_ErrorInfo", "11111"); //�̹� �ִ� ���̵��Դϴ�.
        }
        else if (bro_customSignUp.StatusCode == 400) //��ĭ ����ó��
        {
            UIManager.Instance.UpdateText("Login/Text_ErrorInfo", "22222"); //���̵�� ��й�ȣ�� ��� ĭ�� �����ּ���.
        }
        else //�� �ۿ� ����ó��
        {
            UIManager.Instance.UpdateText("Login/Text_ErrorInfo", "33333"); //Error! �ٽ� �õ����ּ���.
        }
    }
    public void CustomLogin(string id, string pw)
    {
        var bro_customLogin = Backend.BMember.CustomLogin(id, pw);
        if(bro_customLogin.StatusCode == 401) //���̵� or ��й�ȣ�� Ʋ��
        {
            UIManager.Instance.UpdateText("Login/Text_ErrorInfo", "11111"); //���̵� ��й�ȣ�� �ùٸ��� �ʽ��ϴ�.
            return;
        }
        else if (bro_customLogin.StatusCode == 400) //��� ���� ����
        {
            UIManager.Instance.UpdateText("Login/Text_ErrorInfo", "22222"); //����� ������ Ȯ���� �� �����ϴ�. ���� ������ϰų� ����/��ġ�� Ȯ���� �ּ���.�� 
            return;
        }
        else if(!bro_customLogin.IsSuccess()) //�� �ۿ� ����ó��
        {
            UIManager.Instance.UpdateText("Login/Text_ErrorInfo", "33333"); //Error! �ٽ� �õ����ּ���.
            return;
        }

        ErrorInfo errorInfo;
        Backend.Match.JoinMatchMakingServer(out errorInfo); //��ġ����ŷ ���� ����
    }

    public void UpdateNickname(string nickname)
    {
        Debug.Log("�г��� ������ ��û�մϴ�.");
        nickname = nickname.Trim();
        BackendReturnObject bro_checkNickname = Backend.BMember.CheckNicknameDuplication(nickname);
        if (bro_checkNickname.IsSuccess() && bro_checkNickname.StatusCode == 204)
        {
            var bro_updateNickname = Backend.BMember.UpdateNickname(nickname);

            Debug.Log("UpdaateNickName!!!!!");
            Debug.Log(bro_updateNickname.StatusCode);
            Debug.Log(bro_updateNickname.GetStatusCode());////////////////////////

            if (bro_updateNickname.IsSuccess() || bro_updateNickname.StatusCode == 204)
            {
                Debug.Log("�г��� ���濡 �����߽��ϴ� : " + bro_checkNickname);
            }
            else
            {
                Debug.LogError("�г��� ���濡 �����߽��ϴ� : " + bro_checkNickname);
            }
        }
        else if(bro_checkNickname.StatusCode == 400)
        {
            Debug.Log("�г����� �ʹ� ��ϴ�. �ٸ� �г����� �������ּ���.");
        }
        else if (bro_checkNickname.StatusCode == 409)
        {
            Debug.Log("�ߺ��� �г����Դϴ�. �ٸ� �г����� �������ּ���.");
        }
        else
        {
            Debug.Log("���濡 �����߽��ϴ�. �ٽ� �õ����ּ���.");
        }     
    }
}