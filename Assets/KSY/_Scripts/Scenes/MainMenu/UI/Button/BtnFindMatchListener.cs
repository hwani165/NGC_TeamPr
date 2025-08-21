using System;
using UnityEngine;

public class BtnFindMatchListener : MonoBehaviour
{
    private Action OnEnterFindMatch, OnFindedMatch, OnMatchCanceled;
    private void Awake()
    {
        //�α��� �õ� �̺�Ʈ �Ҵ�
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
    
    //��Ȯ�� ����� �޼ҵ� �̸����� ����� ��
    private void EnterFindMatch()
    {
        //��Ī ��û��
    }

    //��Ȯ�� ����� �޼ҵ� �̸����� ����� ��
    private void FindedMatch()
    {
        //��Ī �����
    }

    //��Ȯ�� ����� �޼ҵ� �̸����� ����� ��
    private void MatchCanceled()
    {
        //��Ī ��ҽ�
    }
}
