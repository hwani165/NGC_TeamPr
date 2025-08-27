using System;
using BackEnd;
using BackEnd.Tcp;
using UnityEngine;
public class BackendFunctionMatch : MonoBehaviour
{
    public void FindMatch(Action OnEnterFindMatch, Action OnFindedMatch, Action OnMatchCanceled)
    {
        //유저가 매칭을 신청, 취소 했을 때 그리고 매칭이 성사되었을 때 호출되는 이벤트 핸들러입니다.
        Backend.Match.OnMatchMakingResponse = (MatchMakingResponseEventArgs args) => {

            //매칭 신청 처리
            if (args.ErrInfo == ErrorCode.Match_InProgress) 
            {
                OnEnterFindMatch?.Invoke();
            }

            //매칭 성사 처리
            if (args.MatchCardIndate != null && args.RoomInfo != null) 
            {
                OnFindedMatch?.Invoke();

                string serverAddress = args.RoomInfo.m_inGameServerEndPoint.m_address;
                ushort serverPort = args.RoomInfo.m_inGameServerEndPoint.m_port;
                string roomToken = args.RoomInfo.m_inGameRoomToken;
                bool isReconnecting = false;

                //인게임 서버 접속 
                JoinInGameServer(serverAddress, serverPort, roomToken, isReconnecting);
            }

            //매칭 취소 처리
            if (args.ErrInfo == ErrorCode.Match_MatchMakingCanceled) 
            {
                OnMatchCanceled.Invoke();
            }
        };

        //대기방을 생성하였을 때 호출되는 이벤트 핸들러입니다.
        Backend.Match.OnMatchMakingRoomCreate = (MatchMakingInteractionEventArgs args) =>
        {
            if(args.ErrInfo == ErrorCode.Success)
            {
                //대기실이 생성되면 매칭을 곧바로 시작.
                MatchType random = MatchType.Random;
                MatchModeType oneOnOne = MatchModeType.OneOnOne;
                //매칭 카드 inDate  
                var matchCard = "2025-08-17T13:27:17.823Z";

                //매칭 요청
                Backend.Match.RequestMatchMaking(random, oneOnOne, matchCard);
            }
            else
            {
                //대기방 생성 실패 처리
                Debug.LogError("대기방을 만드는 것에 실패했습니다.");
            }
        };

        //대기방 생성 시도
        Backend.Match.CreateMatchRoom();
    }
    public void JoinInGameServer(string serverAddress, ushort serverPort, string roomToken, bool isReconnecting)
    {
        //유저가 게임방 접속에 성공했을 때 입장한 유저에게만 최초 1회 호출되는 이벤트 핸들러입니다. (+ 재접속시에도 호출됨)
        Backend.Match.OnSessionListInServer += (MatchInGameSessionListEventArgs args) => {

            //게임방 접속 성공 처리
            if (args.ErrInfo == ErrorCode.Success)
            {
                Backend.Match.OnSessionOffline = (MatchInGameSessionEventArgs args) => {
                    GameManager.Instance.EnterAccountMenu();
                };
                GameManager.Instance.EnterInGame();
            }
            //게임방 접속 실패 처리
            else
            {
                Debug.LogError("인게임 룸 접속에 실패했습니다.");
            }
        };

        //인게임 서버에 접속을 성공/실패했을 때 호출되는 이벤트입니다.
        Backend.Match.OnSessionJoinInServer += (JoinArgs) => {
            if (JoinArgs.ErrInfo == ErrorInfo.Success)
            //인게임 서버 접속 성공 처리
            {
                //게임 룸 진입 시도
                Backend.Match.JoinGameRoom(roomToken);
            }
            //인게임 서버 재접속 성공 처리
            else if (JoinArgs.ErrInfo.Category == ErrorCode.Success && JoinArgs.ErrInfo.Detail == ErrorCode.NetworkOnline && JoinArgs.ErrInfo.Reason == "Reconnect Success")
            {
                Debug.Log("인게임 서버 재접속에 성공했습니다.");
            }
            //인게임 서버 접속 실패 처리
            else if (JoinArgs.ErrInfo.Category == ErrorCode.Exception)
            {
                Debug.LogError("인게임 서버 접속에 실패했습니다.");
            }
            //예외 에러처리
            else
            {
                Debug.LogError("Error : To enter in gameserver.");
            }
        };

        //인게임 서버 접속 시도
        Backend.Match.JoinGameServer(serverAddress, serverPort, isReconnecting, out ErrorInfo successInfo);      
    }
    public bool TryReconnect()
    {
        bool isReconnecting = false;
        //재접속 여부 확인
        BackendReturnObject bro_isGameRoomActivate = Backend.Match.IsGameRoomActivate();

        //진행중이었던 게임이 있었을 경우 처리
        if (bro_isGameRoomActivate.StatusCode == 200)
        {
            //진행중이었던 게임방의 정보를 획득
            LitJson.JsonData roomInfo = bro_isGameRoomActivate.GetReturnValuetoJSON();
            string serverAddress = roomInfo["serverPublicHostName"].ToString();
            string roomToken = roomInfo["roomToken"].ToString();
            UInt16 serverPort = Convert.ToUInt16(roomInfo["serverPort"].ToString());
            isReconnecting = true;
            //=> unsigned 16-bit integer

            //획득한 정보를 토대로 재접속 시도
            JoinInGameServer(serverAddress, serverPort, roomToken, isReconnecting);
            return isReconnecting;
        }
        //진행중이었던 게임이 없었을 경우 처리
        else
        {
            return isReconnecting;
        }
    }
}
