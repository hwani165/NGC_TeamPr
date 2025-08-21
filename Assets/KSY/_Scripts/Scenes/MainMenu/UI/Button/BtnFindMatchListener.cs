using System;
using UnityEngine;

public class BtnFindMatchListener : MonoBehaviour
{
    private Action OnEnterFindMatch, OnFindedMatch, OnMatchCanceled;
    private void Awake()
    {
        //로그인 시도 이벤트 할당
        if (OnEnterFindMatch == null)
        {
            OnEnterFindMatch += EnterFindMatch;
        }
        if (OnFindedMatch == null)
        {
            OnFindedMatch += FindedMatch;
        }
        if(OnMatchCanceled == null)
        {
            OnMatchCanceled += MatchCanceled;
        }
    }
    public void FindMatch()
    {
        ServerManager.Instance.FindMatch(OnEnterFindMatch, OnFindedMatch, OnMatchCanceled);
    }
    
    //정확한 기능을 메소드 이름으로 명시할 것
    private void EnterFindMatch()
    {
        //매칭 신청시
    }

    //정확한 기능을 메소드 이름으로 명시할 것
    private void FindedMatch()
    {
        //매칭 성사시
    }

    //정확한 기능을 메소드 이름으로 명시할 것
    private void MatchCanceled()
    {
        //매칭 취소시
    }
}
