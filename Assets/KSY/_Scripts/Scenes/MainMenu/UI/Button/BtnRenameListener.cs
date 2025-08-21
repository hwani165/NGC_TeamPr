using TMPro;
using UnityEngine;

public class BtnRenameListener : MonoBehaviour
{
    [SerializeField] private TMP_InputField inputNickname;
    public void Rename()
    {
        string nickname = inputNickname.text;

        //�г��� ���� �õ�
        int statusCode = ServerManager.Instance.TryUpdateNickName(nickname);

        switch(statusCode)
        {
            //�г��� ���� �õ� ���� ó��
            case 204:
                {
                    break;
                }
            //�г����� 20�� �̻��� ��� ó��
            case 400:
                {
                    break;
                }
            //�ߺ��� �г����� ���� ��� ó��
            case 409:
                {
                    break;
                }
        }
    }
}
