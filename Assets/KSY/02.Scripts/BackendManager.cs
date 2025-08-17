// 뒤끝 SDK namespace 추가
using BackEnd;

public class BackendManager : SingletonBehaviour<BackendManager> //싱글톤 패턴 구현
{
    void Start()
    {
        var bro = Initialize();

        // 뒤끝 초기화에 대한 응답값
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