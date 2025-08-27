using UnityEngine;

public class Btn_FindMatch : MonoBehaviour
{
    public void FindMatch()
    {
        BackendManager.Instance.FindMatch();
    }
}
