using System;
using BackEnd;
using UnityEngine;

public class ServerManager : SingletonBehaviour<ServerManager>
{
    private BFInGame bfInGame;
    private BFAccount bfAccount;
    private BFMatch bfMatch;

    private void Awake()
    {
        base.Awake();
        InitBF();
    }
    private void InitBF()
    {
        if (bfInGame == null && !TryGetComponent(out bfInGame))
        {
            gameObject.AddComponent<BFInGame>();
            bfInGame = GetComponent<BFInGame>();
        }

        if (bfAccount == null && !TryGetComponent(out bfAccount))
        {
            gameObject.AddComponent<BFAccount>();
            bfAccount = GetComponent<BFAccount>();
        }

        if (bfMatch == null && !TryGetComponent(out bfMatch))
        {
            gameObject.AddComponent<BFMatch>();
            bfMatch = GetComponent<BFMatch>();
        }
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
        return bfMatch.TryReconnect();
    }
    public void Login(string id, string pw)
    {
        bfAccount.Login(id,pw);
    }
    public void Login(string id, string pw, Action<bool> OnTryMatchServer, Action<int> OnTryLogin)
    {
        bfAccount.Login(id, pw, OnTryMatchServer, OnTryLogin);
    }
    public int TrySignup(string id, string pw)
    {
        return bfAccount.Signup(id,pw);
    }

    public int TryUpdateNickName(string nickName)
    {
        return bfAccount.UpdateNickname(nickName);
    }

    public void FindMatch(Action OnEnterFindMatch, Action OnFindedMatch, Action OnMatchCanceled)
    {
        bfMatch.FindMatch(OnEnterFindMatch, OnFindedMatch, OnMatchCanceled);
    }
}
