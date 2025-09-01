using System;
using BackEnd;
using BackEnd.Tcp;
using InputData.Player;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour, IReceiver
{
    //User Data
    private UserData _myData = new UserData();
    [SerializeField] private string _nickname;

    //Player
    [SerializeField] private InputActionAsset _inputSetting;
    private MyMovement _myMovement;
    private OtherMovement _otherMovement;
    private string _actionMap = "Player";

    //Debuging
    public TextMeshProUGUI DebugUI;

    //Network
    byte[] receiveBff;

    public void Init()
    {
        //나의 플레이어라면
        if (_nickname == ServerManager.Instance.GetMyData().Value.nickname)
        {
            //Debug.Log("Success : Set MyMovement");

            //입력을 받는 movement 추가
            _myMovement = gameObject.AddComponent<MyMovement>();

            //인풋 시스템 세팅
            PlayerInput input = gameObject.AddComponent<PlayerInput>();
            input.actions = _inputSetting;
            input.defaultActionMap = _actionMap;
            input.actions.Enable();
            input.actions.Enable();
        }
        else
        {
            //Debug.Log("Success : Set OtherMovement");

            //아니라면 수신받는 movement 추가
            _otherMovement = gameObject.AddComponent<OtherMovement>();
        }

        //만약 내가 other (수신만 받는 객체)라면
        if (_otherMovement != null)
        {
            //Debug.Log("Backend.Match.OnMatchRelay");

            //메세지가 브로드 캐스팅 되었을 때 호출 (자기자신 포함)
            Backend.Match.OnMatchRelay += (MatchRelayEventArgs args) =>
            {
                //DebugUI.text = $"sender : {args.From.NickName}\nreceiver : {_nickname}";

                if (args.From.NickName == _nickname)
                {
                    Debug.Log("Receive");

                    //수신 받은 데이터를 버퍼에 담기
                    receiveBff = args.BinaryUserData;
                    var _receiveBff = new Google.FlatBuffers.ByteBuffer(receiveBff);

                    //플레이어 관련 데이터가 맞다면 수신 시도
                    if (PlayerMessage.VerifyPlayerMessage(_receiveBff))
                    {
                        ServerManager.Instance.ReceiveData(_receiveBff, _otherMovement);
                    }
                }
            };
        }
    }

    //플레이어 시작 위치를 결정하고 정보를 넘김
    public void SetPos()
    {
        if (Backend.Match.IsSuperGamer())
        {
            //플레이어 왼쪽 위치 선정
        }
        else
        {
            //플레이어 오른쪽 위치 선정
        }
    }
    public void GetUserData(UserData userData)
    {
        //Debug.Log($"Player Data Set : {userData}");
        //userData 할당
        _myData = userData;

        //할당된 userData를 기반으로 인스턴스 멤버 초기화
        _nickname = _myData.nickname;

        //플레이어 객체에 userData가 할당되었음을 표시
        _myData.hasInit = true;
    }

    //IReceiver
    public virtual void ApplyByteData(byte byteData)
    {
        throw new NotImplementedException("If you want to use this method, you must override it.");
    }
    public virtual void ApplySbyteData(sbyte sbyteData)
    {
        throw new NotImplementedException("If you want to use this method, you must override it.");
    }
    public virtual void ApplySbyteData(sbyte sbyteData1, sbyte sbyteData2, sbyte sbyteData3)
    {
    }
}
