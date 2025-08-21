using System;
using TMPro;
using UnityEngine;
public class BtnLoginListener : MonoBehaviour
{
    private Action<int> OnTryLogin;
    private Action<bool> OnTryEnterMatchServer;

    [SerializeField] private TMP_InputField input_ID;
    [SerializeField] private TMP_InputField input_Pw;
    private void Awake()
    {
        //�α��� �õ� �̺�Ʈ �Ҵ�
        if (OnTryLogin == null)
        {
            OnTryLogin += SuccessLogin;
            OnTryLogin += FailedLogin;
        }
        if (OnTryEnterMatchServer == null)
        {
            OnTryEnterMatchServer += FailedEnterMatchServer;
        }
    }
    public void Login()
    {
        string id = input_ID.text;
        string pw = input_Pw.text;

        //���� ������ �� ������ �ִٸ� ������ �õ�
        if (ServerManager.Instance.TryReconnect()) return;

        //���ٸ� �α��� �õ�
        ServerManager.Instance.Login(id, pw, OnTryEnterMatchServer, OnTryLogin);
    }

    //��Ȯ�� ����� �޼ҵ� �̸����� ����� ��
    private void SuccessLogin(int statusCode)
    {
        //�α��� ���� ó��
        if(statusCode == 200)
        {
            GameManager.Instance.EnterMainMenu();
        }
    }

    //��Ȯ�� ����� �޼ҵ� �̸����� ����� ��
    private void FailedLogin(int statusCode)
    {
        //�α��� ���� ó��
        switch(statusCode)
        {
            //���̵� ��й�ȣ�� Ʋ���� �� ó��
            case 401:
                {
                    UIManager.Instance.UpdateText("Login/ErrorInfo", "Invalid id or password.");
                    break;
                }
            //���ܴ��� ���̵��� ��� ó��
            case 403:
                {
                    UIManager.Instance.UpdateText("Login/ErrorInfo", "This account has been banned.");
                    break;
                }
            //�� �ۿ� ����ó��
            default:
                {
                    //������ �ƴ� ���
                    if(statusCode != 200)
                    UIManager.Instance.UpdateText("Login/ErrorInfo", "Login failed");
                    break;
                }
        }
    }

    //��Ȯ�� ����� �޼ҵ� �̸����� ����� ��
    private void FailedEnterMatchServer(bool isConnected)
    {
        if(isConnected == false)
        {
            UIManager.Instance.UpdateText("Login/ErrorInfo", "Enter MatchServer failed");
        }
    }
}
