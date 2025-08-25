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
    //������ �۽�
    public void SendDataPlayerMovement(Vector2 inputMoveVec, bool inputIsGrounded, bool inputCanDash, bool inputIsDashing)
    {
        //���� ����
        _bufferPlayerMovement.Clear();

        //���� �� ó��
        sbyte moveX = (sbyte)inputMoveVec.x;
        sbyte moveY = (sbyte)inputMoveVec.y;

        //�Ҹ��� �� ó��
        byte flags = 0;

        //�޸� ������ ���� ��Ʈ ����ŷ
        if (inputIsGrounded) flags |= (byte)flagInputPlayerMovement.isGrounded;
        if (inputIsDashing) flags |= (byte)flagInputPlayerMovement.dashing;
        if (inputCanDash) flags |= (byte)flagInputPlayerMovement.canDash;

        //���ۿ� ���̺� ����
        inputPlayerMovement.StartinputPlayerMovement(_bufferPlayerMovement);

        //���ۿ� ����ü ����
        Offset<MoveVec> moveVec = MoveVec.CreateMoveVec(_bufferPlayerMovement, moveX, moveY);

        //���ۿ� ������ �߰�
        inputPlayerMovement.AddMoveVec(_bufferPlayerMovement, moveVec);
        inputPlayerMovement.AddMovementState(_bufferPlayerMovement, flags);

        //������ �߰� ��
        Offset<inputPlayerMovement> offset = inputPlayerMovement.EndinputPlayerMovement(_bufferPlayerMovement);
        _bufferPlayerMovement.Finish(offset.Value);

        byte[] data = _bufferPlayerMovement.SizedByteArray();

        //������ �۽�
        Backend.Match.SendDataToInGameRoom(data);
    }

    //������ ���Ť�
    public void GetDataPlayerMovement(KSY_PlayerMovement data)
    {

    }
}
