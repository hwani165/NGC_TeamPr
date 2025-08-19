using TMPro;
using UnityEngine;

public class Btn_Rename : MonoBehaviour
{
    [SerializeField] private TMP_InputField inputNickname;
    public void Rename()
    {
        string nickname = inputNickname.text;
        BackendManager.Instance.UpdateNickname(nickname);
    }
}
