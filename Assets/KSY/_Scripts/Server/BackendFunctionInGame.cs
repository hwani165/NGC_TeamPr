using System;
using BackEnd;
using Google.FlatBuffers;
using InputData.Platform;
using InputData.Player;
using UnityEngine;

public class BackendFunctionInGame : MonoBehaviour
{
    private readonly FlatBufferBuilder _movementBuilder = new FlatBufferBuilder(32);
    private readonly FlatBufferBuilder _platformStateBuilder = new FlatBufferBuilder(32);
    private readonly FlatBufferBuilder _itemActionBuilder = new FlatBufferBuilder(32);

    [Flags]
    public enum flagPlayerMovementState : byte
    {
        //0
        None = 0b0000,

        //1 
        IsJumping = 0b0001,

        //2
        IsRunning = 0b0010,

        //4
        IsDashing = 0b0100
    }
    [Flags]
    public enum flagPlatformState : byte
    {
        //0
        None = 0b0000,

        //1 
        IsFall = 0b0001,

        //2
        IsOnTimerPlatform = 0b0010,
    }
    [Flags]
    public enum flagPlayerItemState : byte
    {
        //0
        None = 0b0000,

        //1 
        hasItem = 0b0001,

        //2
        isShootingItem = 0b0010,
    }

    //데이터 직렬화
    public byte[] SerializationPlatformStateData(byte id,bool hasPlatformBroken, bool isOnTimerPlatform)
    {
        //버퍼 재사용
        _platformStateBuilder.Clear();

        //비트 마스킹
        byte platformState = 0b0000;

        if (hasPlatformBroken) platformState |= (byte)flagPlatformState.IsFall;
        if (hasPlatformBroken) platformState |= (byte)flagPlatformState.IsOnTimerPlatform;

        //오프셋 세팅 + 데이터 할당
        PlatformMessage.StartPlatformMessage(_platformStateBuilder);

        Offset<PlatformState> offsetStateData = PlatformState.CreatePlatformState(_platformStateBuilder, platformState);
        Offset<PlatformInfo> offsetInfoData = PlatformInfo.CreatePlatformInfo(_platformStateBuilder, id);

        PlatformMessage.AddDataType(_platformStateBuilder, PlatformMessageType.state);
        PlatformMessage.AddSenderInfo(_platformStateBuilder, offsetInfoData);

        Offset<PlatformMessage> offsetResultData = PlatformMessage.EndPlatformMessage(_platformStateBuilder);

        //스키마 버퍼화
        PlatformMessage.FinishPlatformMessageBuffer(_platformStateBuilder, offsetResultData);
        byte[] bff = _platformStateBuilder.SizedByteArray();

        return bff;
    }
    public byte[] SerializationPlayerItemData(bool hasItem, bool isShootingItem)
    {
        //버퍼 재사용
        _itemActionBuilder.Clear();

        //비트 마스킹
        byte State = 0b0000;

        if (hasItem) State |= (byte)flagPlayerItemState.hasItem;
        if (isShootingItem) State |= (byte)flagPlayerItemState.isShootingItem;

        //오프셋 세팅 + 데이터 할당
        Offset<ItemAction> offsetActionData = ItemAction.CreateItemAction(_itemActionBuilder, State);
        Offset<PlayerMessage> offsetResultData = PlayerMessage.CreatePlayerMessage(_itemActionBuilder, PlayerMessageType.item_action, offsetActionData.Value);

        //스키마 버퍼화
        _itemActionBuilder.Finish(offsetResultData.Value);
        byte[] bff = _itemActionBuilder.SizedByteArray();

        return bff;
    }
    public byte[] SerializationPlayerMovementData(sbyte dataMoveX, bool dataIsGrounded, bool dataCanDash, bool dataIsDashing)
    {
        //버퍼 재사용
        _movementBuilder.Clear();

        //이동 방향값
        sbyte moveX = dataMoveX;

        //비트 마스킹
        byte State = 0b0000;

        if (dataIsGrounded) State |= (byte)flagPlayerMovementState.IsJumping;
        if (dataIsDashing) State |= (byte)flagPlayerMovementState.IsRunning;
        if (dataCanDash) State |= (byte)flagPlayerMovementState.IsDashing;

        //오프셋 세팅 + 데이터 할당
        Offset<Movement> offsetMovementData = Movement.CreateMovement(_movementBuilder, State, moveX);
        Offset<PlayerMessage> offsetResultData = PlayerMessage.CreatePlayerMessage(_movementBuilder, PlayerMessageType.movement, offsetMovementData.Value);

        //스키마 버퍼화
        _movementBuilder.Finish(offsetResultData.Value);
        byte[] bff = _movementBuilder.SizedByteArray();

        return bff;
    }

    //데이터 송신
    public void SnedData(byte[] bff)
    {
        Backend.Match.SendDataToInGameRoom(bff);
    }

    //데이터 수신
    public void ReceiveData(ByteBuffer bff, IReceiver Receiver) 
    {
        //버퍼가 비어있다면 반환
        if (bff == null) return;

        //만약 플레이어 관련 메세지라면 처리
        if (PlayerMessage.VerifyPlayerMessage(bff))
        {
            var message = PlayerMessage.GetRootAsPlayerMessage(bff);
            PlayerMessageType playerMessageType = message.DataType;

            //플레이어 관련 메세지일 시 처리
            switch (playerMessageType)
            {
                case PlayerMessageType.movement:
                    {
                        //데이터를 버퍼에서 꺼내옴 (역직렬화)
                        Movement data = message.DataAsmovement();
                        sbyte moveX = data.MoveX;
                        byte movementState = data.MovementState;

                        //데이터를 수신자에게 적용
                        Receiver.ApplySbyteData(moveX);
                        Receiver.ApplyByteData(movementState);
                        break;
                    }

                //플레이어 아이템 액션 수신
                case PlayerMessageType.item_action:
                    {

                        break;
                    }
                default:
                    {
                        Debug.Log("Error");
                        break;
                    }
            }
        }
        //만약 플랫폼 관련 메세지라면 처리
        else if (PlatformMessage.VerifyPlatformMessage(bff))
        {
            var message = PlatformMessage.GetRootAsPlatformMessage(bff);
            PlatformMessageType platfomrMessageType = message.DataType;

            //플랫폼 관련 메세지일 시 처리
            switch (platfomrMessageType)
            {
                case PlatformMessageType.state:
                    {

                        break;
                    }
                default:
                    {
                        Debug.Log("Error");
                        break;
                    }
            }
        }
        else
        {
            Debug.Log("Error");
        }
    }
}
