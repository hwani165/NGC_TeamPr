using System;
using NUnit.Framework;
using UnityEngine;

public interface IReceivable 
{
    public void ApplyData(byte byteData);
    public void ApplyData(sbyte sbyteData);

}
