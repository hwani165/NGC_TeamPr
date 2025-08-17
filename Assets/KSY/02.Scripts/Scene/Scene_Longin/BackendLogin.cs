using System.Collections;
using System.Collections.Generic;
// �ڳ� SDK namespace �߰�
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
        var bro = Backend.BMember.CustomSignUp(id, pw);

        if (bro.IsSuccess()) //���������� ȸ������ �Ϸ�.
        {
            UIManager.Instance.UpdateText("Login/Text_ErrorInfo", "Registration successful");
        }
        else if (bro.GetStatusCode() == "409") //���̵� �ߺ� üũ
        {
            UIManager.Instance.UpdateText("Login/Text_ErrorInfo", "11111"); //�̹� �ִ� ���̵��Դϴ�.
        }
        else if (bro.GetStatusCode() == "400") //��ĭ ����ó��
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
        var bro = Backend.BMember.CustomLogin(id, pw);
        if(bro.GetStatusCode() == "401") //���̵� or ��й�ȣ�� Ʋ��
        {
            UIManager.Instance.UpdateText("Login/Text_ErrorInfo", "11111"); //���̵� ��й�ȣ�� �ùٸ��� �ʽ��ϴ�.
            return;
        }
        else if (bro.GetStatusCode() == "400") //��� ���� ����
        {
            UIManager.Instance.UpdateText("Login/Text_ErrorInfo", "22222"); //����� ������ Ȯ���� �� �����ϴ�. ���� ������ϰų� ����/��ġ�� Ȯ���� �ּ���.�� 
            return;
        }
        else if(!bro.IsSuccess()) //�� �ۿ� ����ó��
        {
            UIManager.Instance.UpdateText("Login/Text_ErrorInfo", "33333"); //Error! �ٽ� �õ����ּ���.
            return;
        }

        ErrorInfo errorInfo;
        bool connected = Backend.Match.JoinMatchMakingServer(out errorInfo); //��ġ����ŷ ���� ����

        if(connected)
        {
            //���� ����
            SceneManager.LoadScene("Scene_MainMenu");
        }
        else
        {
            //���� ����
            UIManager.Instance.UpdateText("Login/Text_ErrorInfo", "44444"); //���� ���ӿ� �����߽��ϴ�. �ٽ� �õ����ּ���.
        }
    }

    public void UpdateNickname(string nickname)
    {
        Debug.Log("�г��� ������ ��û�մϴ�.");

        var bro = Backend.BMember.UpdateNickname(nickname);

        if (bro.IsSuccess())
        {
            Debug.Log("�г��� ���濡 �����߽��ϴ� : " + bro);
        }
        else
        {
            Debug.LogError("�г��� ���濡 �����߽��ϴ� : " + bro);
        }
    }
}