using BackEnd;
using UnityEditor.Experimental.GraphView;
using UnityEditor.MemoryProfiler;
using UnityEngine;

public class Btn_RetryL : MonoBehaviour
{
    public void RetryInitialize()
    {
        BackendReturnObject bro = BackendManager.Instance.Initialize();

        if (bro.IsSuccess())
        {
            Debug.Log("Initalize Success!");
            UIManager.Instance.HideUI("Retry");
        }
        else
        {
            Debug.Log("Initalize failed!");
            UIManager.Instance.ShowUI("Retry");
            UIManager.Instance.UpdateText("Retry/Text_ErrorInfo", "Connection failed. \nPlease try again");
        }
    }
}
