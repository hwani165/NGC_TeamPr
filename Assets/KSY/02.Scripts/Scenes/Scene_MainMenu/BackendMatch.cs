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
        //Backend.Match.CancelMatchMaking(); ��Ī ��û�� ����մϴ�, ��Ī ��û ��Ҵ� ����(���� ���� ����)�� �� �� �ֽ��ϴ�.

        Backend.Match.OnMatchMakingResponse = (MatchMakingResponseEventArgs args) => {

            if (args.ErrInfo == ErrorCode.Match_InProgress) //��Ī ��û ó��
            {
                Debug.Log("��Ī ������....");
            }    

            if (args.MatchCardIndate != null && args.RoomInfo != null) //��Ī ���� ó��
            {
                Debug.Log("��Ī ���� �Ϸ�!");

                string serverAddress = args.RoomInfo.m_inGameServerEndPoint.m_address;
                ushort serverPort = args.RoomInfo.m_inGameServerEndPoint.m_port;
                string roomToken = args.RoomInfo.m_inGameRoomToken;

                JoinInGameServer(serverAddress, serverPort, roomToken, false);
            } 

            if (args.ErrInfo == ErrorCode.Match_MatchMakingCanceled) //��Ī ��� ó��
            {
                Debug.Log("��Ī�� ��ҵǾ����ϴ�.");
            }
                
        };//������ ��Ī�� ��û, ��� ���� �� �׸��� ��Ī�� ����Ǿ��� �� ȣ��Ǵ� �̺�Ʈ �ڵ鷯�Դϴ�.

        Backend.Match.OnMatchMakingRoomCreate = (MatchMakingInteractionEventArgs args) =>
        {
            Backend.Match.RequestMatchMaking(MatchType.Point, MatchModeType.OneOnOne, "2025-08-17T13:27:17.823Z");
        };//������ �����Ǹ� ��Ī�� ��ٷ� ����.

        Backend.Match.CreateMatchRoom();
    }
    public void CreateHostRoom()
    {
        //Backend.Match.OnMatchMakingRoomCreate = (MatchMakingInteractionEventArgs args) =>
        //{
        //    ������ �����Ͽ��� �� ȣ��Ǵ� �̺�Ʈ �ڵ鷯�Դϴ�.
        //};

        //Backend.Match.CreateMatchRoom();

        //�� �ڵ�� �ʴ� and ģ�� �ʴ�
        //ģ�� ��� ����
    }
    public void JoinInGameServer(string serverAddress, ushort serverPort, string roomToken, bool isReconnect) // ������ �Լ�
    {
        Backend.Match.OnSessionListInServer += (MatchInGameSessionListEventArgs args) => {

            if (args.ErrInfo == ErrorCode.Success)
            {
                Debug.Log("�ΰ��� �� ���ӿ� �����߽��ϴ�.");
                SceneManager.LoadScene("Scene_InGame");
            }
            else
            {
                Debug.Log("�ΰ��� �� ���ӿ� �����߽��ϴ�.");
            }
        }; //������ ���ӹ� ���ӿ� �������� �� ������ �������Ը� ���� 1ȸ ȣ��Ǵ� �̺�Ʈ �ڵ鷯�Դϴ�. (+ �����ӽÿ��� ȣ���)
        Backend.Match.OnSessionJoinInServer += (JoinArgs) => {
            if (JoinArgs.ErrInfo == ErrorInfo.Success)
            {
                Debug.Log("�ΰ��� ���� ���ӿ� �����߽��ϴ�.");
                Backend.Match.JoinGameRoom(roomToken);
                //Backend.Match.OnMatchInGameAccess = (MatchInGameSessionEventArgs args) =>
                //{

                //};//������ ���ӹ濡 ������ ������ ȣ��Ǵ� �̺�Ʈ�Դϴ�. (�ڱ� �ڽ� + �ٸ� ��� (��ε� ĳ��Ʈ))
            }
            else if (JoinArgs.ErrInfo.Category == ErrorCode.Success && JoinArgs.ErrInfo.Detail == ErrorCode.NetworkOnline && JoinArgs.ErrInfo.Reason == "Reconnect Success")
            {
                Debug.Log("�ΰ��� ���� �����ӿ� �����߽��ϴ�.");
            }
            else if (JoinArgs.ErrInfo.Category == ErrorCode.Exception)
            {
                Debug.Log("�ΰ��� ���� ���ӿ� �����߽��ϴ�.");
            }
            else
            {
                Debug.Log("Error!");
            }
        };
        ErrorInfo errorInfo = null;

        if (Backend.Match.JoinGameServer(serverAddress, serverPort, isReconnect, out errorInfo) == false)
        {
            Debug.Log("���� ���ῡ �����߽��ϴ�.");
            return;
        }
    }
}
