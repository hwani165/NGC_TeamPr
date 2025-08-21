using System;
using BackEnd;
using BackEnd.Tcp;
using UnityEngine;

public class BFAccount : MonoBehaviour
{
    public void Init()
    {
        Backend.Match.OnJoinMatchMakingServer = (JoinChannelEventArgs joinChannelEventArgs) =>
        {
            //��Ī ���� ���� ���� ó��
            if (joinChannelEventArgs.ErrInfo == ErrorInfo.Success)
            {

            }
            //��Ī ���� ���� ���� ó��
            else
            {

            }
        };
    }
    public void Login(string id, string pw)
    {
        //��Ī ������ ������ ����/�������� �� ȣ��Ǵ� �̺�Ʈ�Դϴ�.
        Backend.Match.OnJoinMatchMakingServer = (JoinChannelEventArgs joinChannelEventArgs) =>
        {
            if (joinChannelEventArgs.ErrInfo == ErrorInfo.Success)
            {
                //��Ī ���� ���� ���� ó��
                GameManager.Instance.EnterMainMenu();
            }
            else
            {
                //��Ī ���� ���� ���� ó��
            }
        };

        //�α��� �õ�
        BackendReturnObject bro_customLogin = Backend.BMember.CustomLogin(id, pw);

        int statusCode = bro_customLogin.StatusCode;

        //�α��� ���� ó��
        if (statusCode == 200)
        {
            //��Ī ���� ���� �õ�
            Backend.Match.JoinMatchMakingServer(out ErrorInfo isSuccess);
        }
        //�α��� ���� ó��
        else { }
    }
    public void Login(string id, string pw, Action<bool> OnTryEnterMatchServer, Action<int> OnTryLogin)
    {
        //��Ī ������ ������ ����/�������� �� ȣ��Ǵ� �̺�Ʈ�Դϴ�.
        Backend.Match.OnJoinMatchMakingServer = (JoinChannelEventArgs joinChannelEventArgs) =>
        {
            //��Ī ���� ���� �õ� �̺�Ʈ ����
            OnTryEnterMatchServer?.Invoke(joinChannelEventArgs.ErrInfo == ErrorInfo.Success);
        };

        //�α��� �õ�
        BackendReturnObject bro_customLogin = Backend.BMember.CustomLogin(id, pw);

        int statusCode = bro_customLogin.StatusCode;

        //�α��� �õ� �̺�Ʈ ����
        OnTryLogin?.Invoke(statusCode);

        //�α��� ������ ��Ī ���� ���� �õ�
        if(statusCode == 200)
        {
            //��Ī ���� ���� �õ�
            Backend.Match.JoinMatchMakingServer(out ErrorInfo isSuccess);
        }

    }
    public int Signup(string id, string pw)
    {
        //ȸ�� ���� �õ�
        BackendReturnObject bro_customSignUp = Backend.BMember.CustomSignUp(id, pw);

        return bro_customSignUp.StatusCode;
    }
    public int UpdateNickname(string nickname)
    {
        //�г��� ���� ���� ����
        nickname = nickname.Trim();

        //�г��� �ߺ� üũ
        BackendReturnObject bro_checkNickname = Backend.BMember.CheckNicknameDuplication(nickname);

        //�г��� �ߺ� üũ ���� ó��
        if(bro_checkNickname.StatusCode == 204)
        {
            //�г��� ���� �õ�
            BackendReturnObject bro_updateNickname = Backend.BMember.UpdateNickname(nickname);

            //�г��� ���� ���� ó��
            if (bro_updateNickname.IsSuccess() || bro_updateNickname.StatusCode == 204)
            {
                Debug.Log("�г��� ���濡 �����߽��ϴ� : " + bro_checkNickname);
                return bro_updateNickname.StatusCode;
            }
            //�г��� ���� ���� ó��
            else
            {
                Debug.LogError("�г��� ���濡 �����߽��ϴ� : " + bro_checkNickname);
            }
        }
        return bro_checkNickname.StatusCode;
    }
}

