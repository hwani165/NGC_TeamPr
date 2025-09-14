using System;
using BackEnd.Tcp;
using TMPro;
using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(Icon))]
public class Icon_Interaction : MonoBehaviour
{
    [SerializeField] private Icon _icon;
    [SerializeField] private TMP_Text _errorInfo;

    private Action<int> TryingLogin;
    private Action<bool> OnTryEnterMatchServer;

    [SerializeField] private TMP_InputField input_id;
    [SerializeField] private TMP_InputField input_pw;
    [SerializeField] private TMP_InputField input_nickname;
    private void Awake()
    {
        //로그인 시도 이벤트 할당
        TryingLogin += _onTryLogin;
    }
    public void QuitGame()
    {
    #if UNITY_EDITOR
            EditorApplication.isPlaying = false; // 에디터에서는 Play 모드 종료
    #else
            Application.Quit(); // 빌드에서는 애플리케이션 종료
    # endif
    }

    public void Signup()
    {
        string id = input_id.text;
        string pw = input_pw.text;
        string nickname = input_nickname.text;

        //입력받은 닉네임이 빈 칸(공백 포함)일 경우 처리
        if (string.IsNullOrWhiteSpace(nickname))
        {
            Debug.LogError("닉네임 칸을 입력해주세요.");
            _icon.IsSuccessWorking = false; 
            return;
        }

        //회원 가입 시도
        int signup_statuscode = Server.Instance.TrySignup(id, pw, nickname);

        switch (signup_statuscode)
        {
            //회원 가입 성공 처리
            case 201:
                {
                    break;
                }
            //빈칸 예외처리
            case 400:
                {
                    _errorInfo.text = "아이디, 비밀번호, 닉네임 칸을 모두 입력해주세요";

                    _icon.IsSuccessWorking = false;
                    return;
                }
            //아이디 중복 처리
            case 409:
                {
                    _errorInfo.text = "중복된 아이디입니다. 다른 아이디로 시도해주세요.";

                    _icon.IsSuccessWorking = false;
                    return;
                }
            //그 밖에 예외처리
            default:
                {
                    _errorInfo.text = "회원 가입에 실패했습니다.";

                    _icon.IsSuccessWorking = false;
                    return;
                }
        }

        //닉네임 변경 시도
        int createNickname_statuscode = Server.Instance.TryUpdateNickname(nickname);

        switch (createNickname_statuscode)
        {
            //닉네임 변경 성공 처리
            case 204:
                {
                    _errorInfo.text = "회원 가입에 성공했습니다.";

                    _icon.IsSuccessWorking = true;
                    return;
                }
            //닉네임이 20자 이상일 경우 처리
            case 400:
                {
                    _errorInfo.text = "닉네임이 너무 깁니다.\n20자 이하의 닉네임으로 시도해주세요.";

                    _icon.IsSuccessWorking = false;
                    return;
                }
            //이미 중복된 닉네임이 있는 경우 처리
            case 409:
                {
                    _errorInfo.text = "중복된 닉네임입니다.\n다른 닉네임으로 시도해주세요.";

                    _icon.IsSuccessWorking = false;
                    return;
                }
            //그 밖에 예외처리
            default:
                {
                    _errorInfo.text = "계정 생성에 실패했습니다. 다시 시도해주세요.";

                    _icon.IsSuccessWorking = false;
                    return;
                }
        }
    }
    public void Login()
    {
        string id = input_id.text;
        string pw = input_pw.text;

        ////만약 재접속 할 게임이 있다면 재접속 시도
        //if (Server.Instance.TryReconnect()) return;

        //없다면 로그인 시도
        Server.Instance.Login(id, pw, TryingLogin);
    }
    public void RetryInitialize()
    {
        if (Server.Instance.TryInitialize())
        {
            //성공 했을 시 UI 숨기기 수정
        }
    }
    public void FindMatch()
    {
        Server.Instance.FindMatch();
    }
    public void Rename()
    {
        string nickname = input_nickname.text;

        //닉네임 변경 시도
        int statusCode = Server.Instance.TryUpdateNickname(nickname);

        switch (statusCode)
        {
            //닉네임 변경 시도 성공 처리
            case 204:
                {
                    _errorInfo.text = $"닉네임 변경에 성공했습니다.\n변경된 닉네임 : {nickname}";
                    GameObject.Find("Canvas/Pages/Image_R_MyInfoPage/Text_MyProfile").GetComponent<TMP_Text>()
                    .text = $"이름 : {Server.Instance.GetMyData().Value.nickname}";
                    return;
                }
            //닉네임이 20자 이상일 경우 처리
            case 400:
                {
                    _errorInfo.text = $"닉네임이 너무 깁니다.\n20자 이하의 닉네임으로 변경해주세요.";
                    break;
                }
            //중복된 닉네임이 있을 경우 처리
            case 409:
                {
                    _errorInfo.text = $"중복된 닉네임입니다. 다른 닉네임으로 변경해주세요.";
                    break;
                }
            default:
                {
                    _errorInfo.text = $"닉네임 변경에 실패했습니다.\n 다시 시도해주세요.";
                    break;
                }
        }
    }

    #region Login event
    private void _onTryLogin(int statusCode)
    {
        //로그인 성공 처리
        if (statusCode == 200)
        {
            _errorInfo.text = "로그인에 성공했습니다.";
            _icon.IsSuccessWorking = true;

            GameObject.Find("Canvas/Pages/Image_R_DefaultPage/Btn_FindMatch").SetActive(true);
            GameObject.Find("Canvas/Pages/Image_R_DefaultPage/Btn_MyInfo").SetActive(true);

            Server.Instance.InitMyData();

            GameObject.Find("Canvas/Pages/Image_R_MyInfoPage/Text_MyProfile").GetComponent<TMP_Text>()
                .text = $"이름 : {Server.Instance.GetMyData().Value.nickname}";
        }
        else
        {
            _icon.IsSuccessWorking = false;

            //로그인 실패 처리
            switch (statusCode)
            {
                //아이디나 비밀번호가 틀렸을 시 처리
                case 401:
                    {
                        _errorInfo.text = "로그인에 실패했습니다\n아이디나 비밀번호를 다시 확인해주세요.";
                        break;
                    }
                //차단당한 아이디일 경우 처리
                case 403:
                    {
                        _errorInfo.text = "로그인에 실패했습니다.\n서버로부터 차단 당한 아이디입니다.";
                        break;
                    }
                //그 밖에 예외처리
                default:
                    {
                        //성공이 아닐 경우
                        if (statusCode != 200)
                        {
                            _errorInfo.text = "로그인에 실패했습니다. 다시 시도해주세요.";
                        }
                        break;
                    }
            }
        }
    }
    #endregion
}


