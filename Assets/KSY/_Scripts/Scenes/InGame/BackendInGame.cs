using BackEnd;
using Google.FlatBuffers;
using MyGame.Player;
using UnityEngine;

public static class BackendInGame
{
    //private void Update()
    //{
    //    //Backend.Match.OnMatchRelay = (MatchRelayEventArgs args) => {
    //    //    //클라이언트가 SendDataToInGameRoom 함수로 서버로 보낸 메시지를 게임방에 접속한
    //    //    //모든 클라이언트에게 브로드캐스팅 했을 때 호출되는 이벤트입니다.
    //    //};
    //}
    public static void SendDataToServer(byte[] binaryData)
    {
        Debug.Log("SendDataToServer");
        Backend.Match.SendDataToInGameRoom(binaryData);
    }
}
