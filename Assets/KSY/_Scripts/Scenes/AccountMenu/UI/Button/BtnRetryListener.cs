using BackEnd;
using UnityEngine;

public class BtnRetryListener : MonoBehaviour
{
    public void RetryInitialize()
    {
        if(ServerManager.Instance.TryInitialize())
        {
            UIManager.Instance.HideUI("RetryInitialize");
        }
    }
}
