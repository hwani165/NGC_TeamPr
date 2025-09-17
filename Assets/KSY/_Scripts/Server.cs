using System;
using System.Xml.Linq;
using BackEnd;
using BackEnd.Tcp;
using InputData.Map;
using InputData.Player;
using UnityEngine;

public class Server : SingletonBehaviour<Server>
{
    private BackendFunctionInGame _bfInGame;
    private BackendFunctionsAccount _bfAccount;
    private BackendFunctionMatch _bfMatch;
    private UserData _myData = new UserData();
    private UserData _otherData = new UserData();
    public static string MyName;
    public static string OtherName;
    public static bool IsSuperGamer { get; private set; } = false;
    private void Awake()
    {
        base.Awake();
        InitBF();
    }
    private void InitBF()
    {
        if (_bfInGame == null && !TryGetComponent(out _bfInGame))
        {
            gameObject.AddComponent<BackendFunctionInGame>();
            _bfInGame = GetComponent<BackendFunctionInGame>();
        }

        if (_bfAccount == null && !TryGetComponent(out _bfAccount))
        {
            gameObject.AddComponent<BackendFunctionsAccount>();
            _bfAccount = GetComponent<BackendFunctionsAccount>();
        }

        if (_bfMatch == null && !TryGetComponent(out _bfMatch))
        {
            gameObject.AddComponent<BackendFunctionMatch>();
            _bfMatch = GetComponent<BackendFunctionMatch>();
        }

        _bfMatch.EnterRoom += () =>
        {
            IsSuperGamer = Backend.Match.IsSuperGamer();
        };
    }
    public void InitOtherData(MatchUserGameRecord otherInfo)
    {
        string nickname = otherInfo.m_nickname;

        //받아왔던 데이터를 할당.
        _otherData.nickname = nickname;

        OtherName = nickname;

        //데이터 초기화를 표시
        _otherData.hasInit = true;
    }
    public void InitMyData()
    {
        //서버에서 내 계정에 맞는 데이터를 가져옴
        var bro_GetUserInfo = Backend.BMember.GetUserInfo();

        //받아온 데이터에서 닉네임을 가져옴
        string nickname = bro_GetUserInfo.GetReturnValuetoJSON()["row"]["nickname"].ToString();

        //받아온 데이터를 할당.
        _myData.nickname = nickname;
        MyName = nickname;

        //데이터 초기화를 표시
        _myData.hasInit = true;
    }
    public UserData? GetMyData()
    {
        //초기화된 값일 경우 반환. 아닐 경우 null 반환
        if (_myData.hasInit == false)
        {
            return null;
        }
        else
        {
            return _myData;
        }
    }
    public UserData? GetOtherData()
    {
        //초기화된 값일 경우 반환. 아닐 경우 null 반환
        if (_otherData.hasInit == false) 
        { 
            return null; 
        }
        else
        {
            return _otherData;
        }
    }
    public void Send(byte[] bff)
    {
        _bfInGame.Send(bff);
    }
    public void ApplyData(PlayerMessage message, IReceiver Receiver, PlayerMessageType tpye)
    {
        _bfInGame.ApplyData(message, Receiver, tpye);
    }
    public void ApplyData(MapMessage message, IReceiver Receiver, MapMessageType type)
    {
        _bfInGame.ApplyData(message, Receiver, type);
    }
    public bool TryInitialize()
    {
        //초기화 시도
        BackendReturnObject bro_Initialize = Backend.Initialize();
        //초기화 성공 처리
        if (bro_Initialize.IsSuccess())
        {
            return true;
        }
        // 초기화 실패 처리
        else
        {
            return false;
        }
    }
    public bool TryReconnect()
    {
        return _bfMatch.TryReconnect();
    }
    public void Login(string id, string pw, Action<int> OnTryLogin)
    {
        _bfAccount.Login(id, pw, OnTryLogin);
    }
    public int TrySignup(string id, string pw , string nickname)
    {
        return _bfAccount.Signup(id,pw, nickname);
    }
    public int TryUpdateNickname(string nickName)
    {
        return _bfAccount.UpdateNickname(nickName);
    }
    public void FindMatch()
    {
        _bfMatch.FindMatch();
    }
    public byte[] SerializationItemDes(ushort id)
    {
        return _bfInGame.SerializationItemDes(id);
    }
    public byte[] SerializationStartEndData(byte mapIndex, sbyte P1LIFE, sbyte P2LIFE)
    {
        return _bfInGame.SerializationEndData(mapIndex, P1LIFE, P2LIFE);
    }
    public byte[] SerializationStartEndData(sbyte hitCount)
    {
        return _bfInGame.SerializationStartEndData(hitCount);
    }
    public byte[] SerializationSpawnerInfoData(ushort spawnItemId, byte spawnItemIndex,byte spawnPotinIndex)
    {
        return _bfInGame.SerializationSpawnerInfoData(spawnItemId, spawnItemIndex, spawnPotinIndex);
    }
    public byte[] SerializationActionData(ushort itemId, bool isThrowing, byte chargeGauge, Vector2 throwDir)
    {
        return _bfInGame.SerializationActionData(itemId, isThrowing, chargeGauge, throwDir);
    }
    public byte[] SerializationCurrentData(string name, sbyte health)
    {
        return _bfInGame.SerializetionCurrentData(name, health);
    }
    public byte[] SerializationPlayerMovementData(sbyte moveX, bool usingJump, bool usingDownDash)
    {
        return _bfInGame.SerializationPlayerMovementData(moveX, usingJump, usingDownDash);
    }
    public byte[] SerializationItemPos(ushort id, Vector2 pos)
    {
        return _bfInGame.SerializationItemPos(id, pos);
    }
    public byte[] SerializationPlayerPos(Vector2 pos)
    {
        return _bfInGame.SerializationPlayerPos(pos);
    }
}


