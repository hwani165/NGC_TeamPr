using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Btn_Login : MonoBehaviour
{
    [SerializeField]  private TMP_InputField input_ID;
    [SerializeField]  private TMP_InputField input_Pw;
    public void Login()
    {
        string id = input_ID.text;
        string pw = input_Pw.text;
    }
}
