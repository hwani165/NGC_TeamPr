using System;
using System.Collections.Generic;
using Google.FlatBuffers;
using InputData.Platform;
using Unity.VisualScripting;
using UnityEngine;

public class Map : MonoBehaviour
{
    //씬에 있는 모든 플랫폼을 담는 배열
    private Dictionary<byte, Platform> _platfomrs;
    private List<Transform> _startPos = new List<Transform>(2);
    private void Start()
    {
        //씬에 있는 모든 플랫폼을 가져옴
        Platform[] platforms = GetComponentsInChildren<Platform>();

        //플랫폼 개수 만큼 딕셔너리 공간 마련
        _platfomrs = new Dictionary<byte, Platform>(platforms.Length);

        //Debug.Log($"{platforms} != null : {platforms != null}");
        //Debug.Log(platforms);

        //모든 플랫폼을 초기화함
        foreach (var platform in platforms)
        {
            platform.Init();
            _platfomrs.Add(platform.Id, platform);
        }
    }
    public void MatchData(ByteBuffer bff)
    {
        //(송신한)수신 받을 플랫폼의 아이디를 찾음
        var message = PlatformMessage.GetRootAsPlatformMessage(bff);
        byte senderId = message.SenderInfo.Value.Id;

        //(송신한)수신 받을 플랫폼을 찾음
        var receiver = _platfomrs[senderId];

        Server.Instance.ReceiveData(bff, receiver);
    }
    internal void SetPos()
    {
        if(BackEnd.Backend.Match.IsSuperGamer())
        {
            transform.position = _startPos[0].position;
        }
        else
        {
            transform.position = _startPos[1].position;
        }
    }
}
