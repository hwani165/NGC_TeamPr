using UnityEngine;

public class Btn_CreateHostRoom : MonoBehaviour
{
    public void CreateHostRoom()
    {
        BackendManager.Instance.CreateHostRoon();
    }
}
