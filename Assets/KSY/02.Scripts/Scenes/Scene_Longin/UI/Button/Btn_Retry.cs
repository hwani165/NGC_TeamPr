using BackEnd;
using UnityEngine;

public class Btn_Retry : MonoBehaviour
{
    public void RetryInitialize()
    {
        BackendManager.Instance.Initialize();
    }
}
