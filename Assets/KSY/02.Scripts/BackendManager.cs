// �ڳ� SDK namespace �߰�
using System;
using System.Collections;
using BackEnd;
using BackEnd.Tcp;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BackendManager : SingletonBehaviour<BackendManager> //�̱��� ���� ����
{
    private bool ussingeMatchMakingServer;
    void Start()
    {
        Initialize();
    }

    private void Update()
    {
        if (ussingeMatchMakingServer)
        {
            Debug.Log("Poll");
            Backend.Match.Poll();
        }
    }

    #region Login
    public void Login(string id, string pw)
    {
        Backend.Match.OnJoinMatchMakingServer = (JoinChannelEventArgs joinChannelEventArgs) =>
        {
            if (joinChannelEventArgs.ErrInfo == ErrorInfo.Success)
            {
                Debug.Log("���� ����");

                BackendReturnObject bro_isGameRoomActivate = Backend.Match.IsGameRoomActivate(); //������ ���� Ȯ��

                if (bro_isGameRoomActivate.StatusCode == 200)
                {
                    Debug.Log("����Ǿ��� ������ ����ϰڽ��ϴ�.");

                    LitJson.JsonData roomInfo = bro_isGameRoomActivate.GetReturnValuetoJSON();
                    string addr = roomInfo["serverPublicHostName"].ToString();
                    string roomToken = roomInfo["roomToken"].ToString();
                    UInt16 port = Convert.ToUInt16(roomInfo["serverPort"].ToString());
                    //=> unsigned 16-bit integer

                    BackendMatch.Instance.JoinInGameServer(addr, port, roomToken, true);
                    return;
                }

                SceneManager.LoadScene("Scene_MainMenu");
            }
            else
            {
                Debug.Log("���� ����");
                Debug.Log(joinChannelEventArgs.ErrInfo.Reason);
                UIManager.Instance.UpdateText("Login/Text_ErjororInfo", "44444");
            }
        }; //��Ī ������ ������ ����/�������� �� ȣ��Ǵ� �̺�Ʈ�Դϴ�.
        #region ��Ī ���� ���� ����
        //Backend.Match.LeaveMatchMakingServer(); ��Ī ������ ���� ���Ḧ �õ��մϴ�.

        //Backend.Match.OnLeaveMatchMakingServer = (LeaveChannelEventArgs args) =>
        //{
        //    //��Ī ������ ������ ����Ǿ��� �� ȣ��Ǵ� �̺�Ʈ�Դϴ�.
        //};
        #endregion

        ussingeMatchMakingServer = true;
        BackendLogin.Instance.CustomLogin(id, pw);
    }

    public void Signup(string id, string pw)
    {
        BackendLogin.Instance.CustomSignUp(id, pw);
    }

    public void UpdateNickname(string newNickname)
    {
        BackendLogin.Instance.UpdateNickname(newNickname);
    }

    public BackendReturnObject Initialize()
    {
        BackendReturnObject bro = Backend.Initialize();

        if (bro.IsSuccess())
        {
            UIManager.Instance.HideUI("Retry");
        }
        else
        {
            UIManager.Instance.ShowUI("Retry");
            UIManager.Instance.UpdateText("Retry/Text_ErrorInfo", "Connection failed. \nPlease try again");
        }

        return bro;
    }
    #endregion

    #region Match
    public void FindMatch()
    {
        BackendMatch.Instance.FindMatch();
        
    }

    public void CreateHostRoon()
    {
        BackendMatch.Instance.CreateHostRoom();
    }

    public void JoinHostRoom()
    {

    }
    #endregion

}