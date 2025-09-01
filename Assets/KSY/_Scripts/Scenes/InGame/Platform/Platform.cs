using System;
using UnityEngine;

public class Platform : MonoBehaviour, IReceiver
{
    protected int id;
    public virtual void ApplyByteData(byte byteData)
    {
        throw new NotImplementedException("If you want to use this method, you must override it.");
    }
    public virtual void ApplySbyteData(sbyte sbyteData)
    {
        throw new NotImplementedException("If you want to use this method, you must override it.");
    }
}
