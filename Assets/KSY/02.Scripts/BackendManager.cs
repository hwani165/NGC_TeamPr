// 뒤끝 SDK namespace 추가
using System;
using System.Collections;
using BackEnd;
using BackEnd.Tcp;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BackendManager : SingletonBehaviour<BackendManager> //싱글톤 패턴 구현
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
                Debug.Log("접속 성공");

                BackendReturnObject bro_isGameRoomActivate = Backend.Match.IsGameRoomActivate(); //재접속 여부 확인

                if (bro_isGameRoomActivate.StatusCode == 200)
                {
                    Debug.Log("진행되었던 게임을 계속하겠습니다.");

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
                Debug.Log("접속 실패");
                Debug.Log(joinChannelEventArgs.ErrInfo.Reason);
                UIManager.Instance.UpdateText("Login/Text_ErjororInfo", "44444");
            }
        }; //매칭 서버에 접속을 성공/실패했을 때 호출되는 이벤트입니다.
        #region 매칭 서버 접속 종료
        //Backend.Match.LeaveMatchMakingServer(); 매칭 서버와 접속 종료를 시도합니다.

        //Backend.Match.OnLeaveMatchMakingServer = (LeaveChannelEventArgs args) =>
        //{
        //    //매칭 서버와 접속이 종료되었을 때 호출되는 이벤트입니다.
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