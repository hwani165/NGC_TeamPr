using System.Collections;
using BackEnd;
using BackEnd.Tcp;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BackendMatch
{
    private static BackendMatch _instance = null;
    public static BackendMatch Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new BackendMatch();
            }

            return _instance;
        }
    }

    public void FindMatch()
    {
        //Backend.Match.CancelMatchMaking(); 매칭 신청을 취소합니다, 매칭 신청 취소는 방장(방을 만든 유저)만 할 수 있습니다.

        Backend.Match.OnMatchMakingResponse = (MatchMakingResponseEventArgs args) => {

            if (args.ErrInfo == ErrorCode.Match_InProgress) //매칭 신청 처리
            {
                Debug.Log("매칭 진행중....");
            }    

            if (args.MatchCardIndate != null && args.RoomInfo != null) //매칭 성사 처리
            {
                Debug.Log("매칭 성사 완료!");

                string serverAddress = args.RoomInfo.m_inGameServerEndPoint.m_address;
                ushort serverPort = args.RoomInfo.m_inGameServerEndPoint.m_port;
                string roomToken = args.RoomInfo.m_inGameRoomToken;

                JoinInGameServer(serverAddress, serverPort, roomToken, false);
            } 

            if (args.ErrInfo == ErrorCode.Match_MatchMakingCanceled) //매칭 취소 처리
            {
                Debug.Log("매칭이 취소되었습니다.");
            }
                
        };//유저가 매칭을 신청, 취소 했을 때 그리고 매칭이 성사되었을 때 호출되는 이벤트 핸들러입니다.

        Backend.Match.OnMatchMakingRoomCreate = (MatchMakingInteractionEventArgs args) =>
        {
            Backend.Match.RequestMatchMaking(MatchType.Point, MatchModeType.OneOnOne, "2025-08-17T13:27:17.823Z");
        };//대기실이 생성되면 매칭을 곧바로 시작.

        Backend.Match.CreateMatchRoom();
    }
    public void CreateHostRoom()
    {
        //Backend.Match.OnMatchMakingRoomCreate = (MatchMakingInteractionEventArgs args) =>
        //{
        //    대기방을 생성하였을 때 호출되는 이벤트 핸들러입니다.
        //};

        //Backend.Match.CreateMatchRoom();

        //방 코드로 초대 and 친구 초대
        //친구 기능 구현
    }
    public void JoinInGameServer(string serverAddress, ushort serverPort, string roomToken, bool isReconnect) // 임의의 함수
    {
        Backend.Match.OnSessionListInServer += (MatchInGameSessionListEventArgs args) => {

            if (args.ErrInfo == ErrorCode.Success)
            {
                Debug.Log("인게임 룸 접속에 성공했습니다.");
                SceneManager.LoadScene("Scene_InGame");
            }
            else
            {
                Debug.Log("인게임 룸 접속에 실패했습니다.");
            }
        }; //유저가 게임방 접속에 성공했을 때 입장한 유저에게만 최초 1회 호출되는 이벤트 핸들러입니다. (+ 재접속시에도 호출됨)
        Backend.Match.OnSessionJoinInServer += (JoinArgs) => {
            if (JoinArgs.ErrInfo == ErrorInfo.Success)
            {
                Debug.Log("인게임 서버 접속에 성공했습니다.");
                Backend.Match.JoinGameRoom(roomToken);
                //Backend.Match.OnMatchInGameAccess = (MatchInGameSessionEventArgs args) =>
                //{

                //};//유저가 게임방에 입장할 때마다 호출되는 이벤트입니다. (자기 자신 + 다른 사람 (브로드 캐스트))
            }
            else if (JoinArgs.ErrInfo.Category == ErrorCode.Success && JoinArgs.ErrInfo.Detail == ErrorCode.NetworkOnline && JoinArgs.ErrInfo.Reason == "Reconnect Success")
            {
                Debug.Log("인게임 서버 재접속에 성공했습니다.");
            }
            else if (JoinArgs.ErrInfo.Category == ErrorCode.Exception)
            {
                Debug.Log("인게임 서버 접속에 실패했습니다.");
            }
            else
            {
                Debug.Log("Error!");
            }
        };
        ErrorInfo errorInfo = null;

        if (Backend.Match.JoinGameServer(serverAddress, serverPort, isReconnect, out errorInfo) == false)
        {
            Debug.Log("서버 연결에 실패했습니다.");
            return;
        }
    }
}
