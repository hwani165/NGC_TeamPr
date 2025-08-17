// �ڳ� SDK namespace �߰�
using BackEnd;

public class BackendManager : SingletonBehaviour<BackendManager> //�̱��� ���� ����
{
    void Start()
    {
        var bro = Initialize();

        // �ڳ� �ʱ�ȭ�� ���� ���䰪
        if (!bro.IsSuccess())
        {
            UIManager.Instance.ShowUI("Retry");
        }
    }
    public BackendReturnObject Initialize()
    {
        BackendReturnObject bro = Backend.Initialize(); 
        return bro;
    }
}