using System;
using TMPro;
using UnityEditor;
using UnityEngine;

public class Btn_Interaction : MonoBehaviour
{
    private Action<int> OnTryLogin;
    private Action<bool> OnTryEnterMatchServer;

    [SerializeField] private TMP_InputField input_ID;
    [SerializeField] private TMP_InputField input_Pw;
    [SerializeField] private TMP_InputField input_Nickname;
    private void Awake()
    {
        //로그인 시도 이벤트 할당
        OnTryLogin += _onSuccessLogin;
        OnTryLogin += _onFiledLogin;

        OnTryEnterMatchServer += _onFailedEnterMatchServer;
    }
    public void _quitGame()
    {
    #if UNITY_EDITOR
            EditorApplication.isPlaying = false; // 에디터에서는 Play 모드 종료
    #else
            Application.Quit(); // 빌드에서는 애플리케이션 종료
    # endif
    }

    public void Signup()
    {
        string id = input_ID.text;
        string pw = input_Pw.text;
        string nickname = input_Nickname.text;

        //입력받은 닉네임이 빈 칸(공백 포함)일 경우 처리
        if (string.IsNullOrWhiteSpace(nickname))
        {
            Debug.Log("닉네임이나 아이디를 모두 입력해주세요.");
            //UIManager.Instance.UpdateText("Login/Text_ErrorInfo", "121212");
            return;
        }

        //회원 가입 시도
        int signup_statusCode = Server.Instance.TrySignup(id, pw, nickname);

        switch (signup_statusCode)
        {
            //회원 가입 성공 처리
            case 201:
                {
                    //UIManager.Instance.UpdateText("Login/Text_ErrorInfo", "Registration successful");
                    break;
                }
            //빈칸 예외처리
            case 400:
                {
                    //text : 아이디와 비밀번호를 모두 칸에 적어주세요.
                    //UIManager.Instance.UpdateText("Login/Text_ErrorInfo", "22222");
                    break;
                }
            //아이디 중복 처리
            case 409:
                {
                    //UIManager.Instance.UpdateText("Login/Text_ErrorInfo", "11111");
                    break;
                }
            //그 밖에 예외처리
            default:
                {
                    //text : Error ! 다시 시도해주세요.
                    //UIManager.Instance.UpdateText("Login/Text_ErrorInfo", "33333");
                    break;
                }
        }

        //닉네임 변경 시도
        int updateNicknameStatuscode = Server.Instance.TryUpdateNickname(nickname);

        switch (updateNicknameStatuscode)
        {
            //닉네임 변경 성공 처리
            case 204:
                {
                    //UIManager.Instance.UpdateText("Login/Text_ErrorInfo", "Registration successful");
                    break;
                }
            //닉네임이 20자 이상일 경우 처리
            case 400:
                {
                    Debug.LogError("닉네임이 너무 깁니다. 다른 닉네임을 선택해주세요.");
                    break;
                }
            //이미 중복된 닉네임이 있는 경우 처리
            case 409:
                {
                    Debug.LogError("중복된 닉네임입니다. 다른 닉네임을 선택해주세요.");
                    break;
                }
            //그 밖에 예외처리
            default:
                {
                    //text : Error ! 다시 시도해주세요.
                    //UIManager.Instance.UpdateText("Login/Text_ErrorInfo", "33333");
                    break;
                }
        }
    }
    public void Login()
    {
        string id = input_ID.text;
        string pw = input_Pw.text;

        //만약 재접속 할 게임이 있다면 재접속 시도
        if (Server.Instance.TryReconnect()) return;

        //없다면 로그인 시도
        Server.Instance.Login(id, pw, OnTryEnterMatchServer, OnTryLogin);
    }
    #region Login event
    private void _onSuccessLogin(int statusCode)
    {
        //로그인 성공 처리
        if (statusCode == 200)
        {
            Server.Instance.InitMyData();
            Game.Instance.EnterScene(SceneType.MainMenu);
        }

        //이벤트 할당 해제
        OnTryLogin -= _onSuccessLogin;
    }

    //정확한 기능을 메소드 이름으로 명시할 것
    private void _onFiledLogin(int statusCode)
    {
        //로그인 실패 처리
        switch (statusCode)
        {
            //아이디나 비밀번호가 틀렸을 시 처리
            case 401:
                {
                    //UIManager.Instance.UpdateText("Login/ErrorInfo", "Invalid id or password.");
                    break;
                }
            //차단당한 아이디일 경우 처리
            case 403:
                {
                    //UIManager.Instance.UpdateText("Login/ErrorInfo", "This account has been banned.");
                    break;
                }
            //그 밖에 예외처리
            default:
                {
                    //성공이 아닐 경우
                    if (statusCode != 200)
                    {
                        //UIManager.Instance.UpdateText("Login/ErrorInfo", "Login failed");
                    }
                    break;
                }
        }

        //이벤트 할당 해제
        OnTryLogin -= _onFiledLogin;
    }

    //수정
    private void _onFailedEnterMatchServer(bool isConnected)
    {
        if (isConnected == false)
        {
            Debug.Log("Not Success : Enter Match Server");
        }
        else
        {
            Debug.Log("Success : Enter Match Server !");
        }
    }
    #endregion
    public void RetryInitialize()
    {
        if (Server.Instance.TryInitialize())
        {
            //성공 했을 시 UI 숨기기 수정
        }
    }
}


