using BackEnd;
using Google.FlatBuffers;
using MyGame.Player;
using UnityEngine;

public static class BackendInGame
{
    //private void Update()
    //{
    //    //Backend.Match.OnMatchRelay = (MatchRelayEventArgs args) => {
    //    //    //Ŭ���̾�Ʈ�� SendDataToInGameRoom �Լ��� ������ ���� �޽����� ���ӹ濡 ������
    //    //    //��� Ŭ���̾�Ʈ���� ��ε�ĳ���� ���� �� ȣ��Ǵ� �̺�Ʈ�Դϴ�.
    //    //};
    //}
    public static void SendDataToServer(byte[] binaryData)
    {
        Debug.Log("SendDataToServer");
        Backend.Match.SendDataToInGameRoom(binaryData);
    }
}
