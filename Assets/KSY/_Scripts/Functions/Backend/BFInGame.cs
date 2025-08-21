using UnityEngine;
using Google.FlatBuffers;

public class BFInGame : MonoBehaviour
{
    FlatBufferBuilder flatBufferBuilder = new FlatBufferBuilder(1024);

    public void test()
    {
        StringOffset nameOffset = flatBufferBuilder.CreateString("Orc");
    }
}
