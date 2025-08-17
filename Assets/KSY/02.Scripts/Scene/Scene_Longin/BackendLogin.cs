using System.Collections;
using System.Collections.Generic;
// 뒤끝 SDK namespace 추가
using BackEnd;
using BackEnd.Tcp;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BackendLogin
{
    private static BackendLogin _instance = null;
    public static BackendLogin Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new BackendLogin();
            }

            return _instance;
        }
    }

    public void CustomSignUp(string id, string pw)
    {
        var bro = Backend.BMember.CustomSignUp(id, pw);

        if (bro.IsSuccess()) //성공적으로 회원가입 완료.
        {
            UIManager.Instance.UpdateText("Login/Text_ErrorInfo", "Registration successful");
        }
        else if (bro.GetStatusCode() == "409") //아이디 중복 체크
        {
            UIManager.Instance.UpdateText("Login/Text_ErrorInfo", "11111"); //이미 있는 아이디입니다.
        }
        else if (bro.GetStatusCode() == "400") //빈칸 예외처리
        {
            UIManager.Instance.UpdateText("Login/Text_ErrorInfo", "22222"); //아이디와 비밀번호를 모두 칸에 적어주세요.
        }
        else //그 밖에 예외처리
        {
            UIManager.Instance.UpdateText("Login/Text_ErrorInfo", "33333"); //Error! 다시 시도해주세요.
        }
    }

    public void CustomLogin(string id, string pw)
    {
        var bro = Backend.BMember.CustomLogin(id, pw);
        if(bro.GetStatusCode() == "401") //아이디 or 비밀번호가 틀림
        {
            UIManager.Instance.UpdateText("Login/Text_ErrorInfo", "11111"); //아이디나 비밀번호가 올바르지 않습니다.
            return;
        }
        else if (bro.GetStatusCode() == "400") //기기 정보 누락
        {
            UIManager.Instance.UpdateText("Login/Text_ErrorInfo", "22222"); //“기기 정보를 확인할 수 없습니다. 앱을 재실행하거나 권한/설치를 확인해 주세요.” 
            return;
        }
        else if(!bro.IsSuccess()) //그 밖에 예외처리
        {
            UIManager.Instance.UpdateText("Login/Text_ErrorInfo", "33333"); //Error! 다시 시도해주세요.
            return;
        }

        ErrorInfo errorInfo;
        bool connected = Backend.Match.JoinMatchMakingServer(out errorInfo); //매치메이킹 서버 접속

        if(connected)
        {
            //연결 성공
            SceneManager.LoadScene("Scene_MainMenu");
        }
        else
        {
            //연결 실패
            UIManager.Instance.UpdateText("Login/Text_ErrorInfo", "44444"); //서버 접속에 실패했습니다. 다시 시도해주세요.
        }
    }

    public void UpdateNickname(string nickname)
    {
        Debug.Log("닉네임 변경을 요청합니다.");

        var bro = Backend.BMember.UpdateNickname(nickname);

        if (bro.IsSuccess())
        {
            Debug.Log("닉네임 변경에 성공했습니다 : " + bro);
        }
        else
        {
            Debug.LogError("닉네임 변경에 실패했습니다 : " + bro);
        }
    }
}