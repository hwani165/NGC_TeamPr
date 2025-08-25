using System;
using BackEnd;
using BackEnd.Tcp;
using UnityEngine;
public class BackendFunctionMatch : MonoBehaviour
{
    public void FindMatch(Action OnEnterFindMatch, Action OnFindedMatch, Action OnMatchCanceled)
    {
        //������ ��Ī�� ��û, ��� ���� �� �׸��� ��Ī�� ����Ǿ��� �� ȣ��Ǵ� �̺�Ʈ �ڵ鷯�Դϴ�.
        Backend.Match.OnMatchMakingResponse = (MatchMakingResponseEventArgs args) => {

            //��Ī ��û ó��
            if (args.ErrInfo == ErrorCode.Match_InProgress) 
            {
                OnEnterFindMatch?.Invoke();
            }

            //��Ī ���� ó��
            if (args.MatchCardIndate != null && args.RoomInfo != null) 
            {
                OnFindedMatch?.Invoke();

                string serverAddress = args.RoomInfo.m_inGameServerEndPoint.m_address;
                ushort serverPort = args.RoomInfo.m_inGameServerEndPoint.m_port;
                string roomToken = args.RoomInfo.m_inGameRoomToken;
                bool isReconnecting = false;

                //�ΰ��� ���� ���� 
                JoinInGameServer(serverAddress, serverPort, roomToken, isReconnecting);
            }

            //��Ī ��� ó��
            if (args.ErrInfo == ErrorCode.Match_MatchMakingCanceled) 
            {
                OnMatchCanceled.Invoke();
            }
        };

        //������ �����Ͽ��� �� ȣ��Ǵ� �̺�Ʈ �ڵ鷯�Դϴ�.
        Backend.Match.OnMatchMakingRoomCreate = (MatchMakingInteractionEventArgs args) =>
        {
            if(args.ErrInfo == ErrorCode.Success)
            {
                //������ �����Ǹ� ��Ī�� ��ٷ� ����.
                MatchType random = MatchType.Random;
                MatchModeType oneOnOne = MatchModeType.OneOnOne;
                //��Ī ī�� inDate  
                var matchCard = "2025-08-17T13:27:17.823Z";

                //��Ī ��û
                Backend.Match.RequestMatchMaking(random, oneOnOne, matchCard);
            }
            else
            {
                //���� ���� ���� ó��
                Debug.LogError("������ ����� �Ϳ� �����߽��ϴ�.");
            }
        };

        //���� ���� �õ�
        Backend.Match.CreateMatchRoom();
    }
    public void JoinInGameServer(string serverAddress, ushort serverPort, string roomToken, bool isReconnecting)
    {
        //������ ���ӹ� ���ӿ� �������� �� ������ �������Ը� ���� 1ȸ ȣ��Ǵ� �̺�Ʈ �ڵ鷯�Դϴ�. (+ �����ӽÿ��� ȣ���)
        Backend.Match.OnSessionListInServer += (MatchInGameSessionListEventArgs args) => {

            //���ӹ� ���� ���� ó��
            if (args.ErrInfo == ErrorCode.Success)
            {
                Backend.Match.OnSessionOffline = (MatchInGameSessionEventArgs args) => {
                    GameManager.Instance.EnterAccountMenu();
                };
                GameManager.Instance.EnterInGame();
            }
            //���ӹ� ���� ���� ó��
            else
            {
                Debug.LogError("�ΰ��� �� ���ӿ� �����߽��ϴ�.");
            }
        };

        //�ΰ��� ������ ������ ����/�������� �� ȣ��Ǵ� �̺�Ʈ�Դϴ�.
        Backend.Match.OnSessionJoinInServer += (JoinArgs) => {
            if (JoinArgs.ErrInfo == ErrorInfo.Success)
            //�ΰ��� ���� ���� ���� ó��
            {
                //���� �� ���� �õ�
                Backend.Match.JoinGameRoom(roomToken);
            }
            //�ΰ��� ���� ������ ���� ó��
            else if (JoinArgs.ErrInfo.Category == ErrorCode.Success && JoinArgs.ErrInfo.Detail == ErrorCode.NetworkOnline && JoinArgs.ErrInfo.Reason == "Reconnect Success")
            {
                Debug.Log("�ΰ��� ���� �����ӿ� �����߽��ϴ�.");
            }
            //�ΰ��� ���� ���� ���� ó��
            else if (JoinArgs.ErrInfo.Category == ErrorCode.Exception)
            {
                Debug.LogError("�ΰ��� ���� ���ӿ� �����߽��ϴ�.");
            }
            //���� ����ó��
            else
            {
                Debug.LogError("Error : To enter in gameserver.");
            }
        };

        //�ΰ��� ���� ���� �õ�
        Backend.Match.JoinGameServer(serverAddress, serverPort, isReconnecting, out ErrorInfo successInfo);      
    }
    public bool TryReconnect()
    {
        bool isReconnecting = false;
        //������ ���� Ȯ��
        BackendReturnObject bro_isGameRoomActivate = Backend.Match.IsGameRoomActivate();

        //�������̾��� ������ �־��� ��� ó��
        if (bro_isGameRoomActivate.StatusCode == 200)
        {
            //�������̾��� ���ӹ��� ������ ȹ��
            LitJson.JsonData roomInfo = bro_isGameRoomActivate.GetReturnValuetoJSON();
            string serverAddress = roomInfo["serverPublicHostName"].ToString();
            string roomToken = roomInfo["roomToken"].ToString();
            UInt16 serverPort = Convert.ToUInt16(roomInfo["serverPort"].ToString());
            isReconnecting = true;
            //=> unsigned 16-bit integer

            //ȹ���� ������ ���� ������ �õ�
            JoinInGameServer(serverAddress, serverPort, roomToken, isReconnecting);
            return isReconnecting;
        }
        //�������̾��� ������ ������ ��� ó��
        else
        {
            return isReconnecting;
        }
    }
}
