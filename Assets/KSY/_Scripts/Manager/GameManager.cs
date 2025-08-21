using BackEnd;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : SingletonBehaviour<GameManager>
{
    private void Awake()
    {
        BackendReturnObject Initialize = Backend.Initialize();

        if (!Initialize.IsSuccess())
        {
            //�ʱ�ȭ�� �������� �� ó��
            UIManager.Instance.ShowUI("Retry");
            UIManager.Instance.UpdateText("Retry/Text_ErrorInfo", "Connection failed. \nPlease try again");
        }
    }
    private void Update()
    {
       Debug.Log("Poll");
       Backend.Match.Poll();
    }

    public void EnterAccountMenu()
    {
        SceneManager.LoadScene("AccountMenu");
    }
    public void EnterMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
    public void EnterInGame()
    {
        SceneManager.LoadScene("InGame");
    }
    public void ExitAccountMenu()
    {

    }
    public void ExitMainMenu()
    {

    }
    public void ExitInGame()
    {

    }
}
