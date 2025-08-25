using System;
using System.Threading;
using BackEnd;
using Google.FlatBuffers;
using MyGame.Player;
using UnityEngine;

public class BackendFunctionInGame : MonoBehaviour
{
    private const byte FLAG_GROUNDED = 1 << 0; // 0000 0001 (1)
    private byte FLAG_DASHING = 1 << 1; // 0000 0010 (2)
    private const byte FLAG_CANDASH = 1 << 2; // 0000 0100 (4)

    private readonly FlatBufferBuilder _bufferPlayerMovement = new FlatBufferBuilder(128);
    private Offset<inputPlayerMovement> _resultBufferPlayerMovement;

    [Flags]
    public enum flagInputPlayerMovement : byte
    {
        None = 0,
        //1 
        isGrounded = 1 << 0,   
        
        //2
        dashing = 1 << 1,  
        
        //4
        canDash = 1 << 2
    }
    //데이터 송신
    public void SendDataPlayerMovement(Vector2 inputMoveVec, bool inputIsGrounded, bool inputCanDash, bool inputIsDashing)
    {
        //버퍼 재사용
        _bufferPlayerMovement.Clear();

        //벡터 값 처리
        sbyte moveX = (sbyte)inputMoveVec.x;
        sbyte moveY = (sbyte)inputMoveVec.y;

        //불리언 값 처리
        byte flags = 0;

        //메모리 절약을 위한 비트 마스킹
        if (inputIsGrounded) flags |= (byte)flagInputPlayerMovement.isGrounded;
        if (inputIsDashing) flags |= (byte)flagInputPlayerMovement.dashing;
        if (inputCanDash) flags |= (byte)flagInputPlayerMovement.canDash;

        //버퍼에 테이블 생성
        inputPlayerMovement.StartinputPlayerMovement(_bufferPlayerMovement);

        //버퍼에 구조체 생성
        Offset<MoveVec> moveVec = MoveVec.CreateMoveVec(_bufferPlayerMovement, moveX, moveY);

        //버퍼에 데이터 추가
        inputPlayerMovement.AddMoveVec(_bufferPlayerMovement, moveVec);
        inputPlayerMovement.AddMovementState(_bufferPlayerMovement, flags);

        //데이터 추가 끝
        Offset<inputPlayerMovement> offset = inputPlayerMovement.EndinputPlayerMovement(_bufferPlayerMovement);
        _bufferPlayerMovement.Finish(offset.Value);

        byte[] data = _bufferPlayerMovement.SizedByteArray();

        //데이터 송신
        Backend.Match.SendDataToInGameRoom(data);
    }

    //데이터 수신ㄴ
    public void GetDataPlayerMovement(KSY_PlayerMovement data)
    {

    }
}
