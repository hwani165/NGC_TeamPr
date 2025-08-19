using System.Collections;
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
        var bro_customSignUp = Backend.BMember.CustomSignUp(id, pw);

        if (bro_customSignUp.IsSuccess()) //성공적으로 회원가입 완료.
        {
            UIManager.Instance.UpdateText("Login/Text_ErrorInfo", "Registration successful");
        }
        else if (bro_customSignUp.StatusCode == 409) //아이디 중복 체크
        {
            UIManager.Instance.UpdateText("Login/Text_ErrorInfo", "11111"); //이미 있는 아이디입니다.
        }
        else if (bro_customSignUp.StatusCode == 400) //빈칸 예외처리
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
        var bro_customLogin = Backend.BMember.CustomLogin(id, pw);
        if(bro_customLogin.StatusCode == 401) //아이디 or 비밀번호가 틀림
        {
            UIManager.Instance.UpdateText("Login/Text_ErrorInfo", "11111"); //아이디나 비밀번호가 올바르지 않습니다.
            return;
        }
        else if (bro_customLogin.StatusCode == 400) //기기 정보 누락
        {
            UIManager.Instance.UpdateText("Login/Text_ErrorInfo", "22222"); //“기기 정보를 확인할 수 없습니다. 앱을 재실행하거나 권한/설치를 확인해 주세요.” 
            return;
        }
        else if(!bro_customLogin.IsSuccess()) //그 밖에 예외처리
        {
            UIManager.Instance.UpdateText("Login/Text_ErrorInfo", "33333"); //Error! 다시 시도해주세요.
            return;
        }

        ErrorInfo errorInfo;
        Backend.Match.JoinMatchMakingServer(out errorInfo); //매치메이킹 서버 접속
    }

    public void UpdateNickname(string nickname)
    {
        Debug.Log("닉네임 변경을 요청합니다.");
        nickname = nickname.Trim();
        BackendReturnObject bro_checkNickname = Backend.BMember.CheckNicknameDuplication(nickname);
        if (bro_checkNickname.IsSuccess() && bro_checkNickname.StatusCode == 204)
        {
            var bro_updateNickname = Backend.BMember.UpdateNickname(nickname);

            Debug.Log("UpdaateNickName!!!!!");
            Debug.Log(bro_updateNickname.StatusCode);
            Debug.Log(bro_updateNickname.GetStatusCode());////////////////////////

            if (bro_updateNickname.IsSuccess() || bro_updateNickname.StatusCode == 204)
            {
                Debug.Log("닉네임 변경에 성공했습니다 : " + bro_checkNickname);
            }
            else
            {
                Debug.LogError("닉네임 변경에 실패했습니다 : " + bro_checkNickname);
            }
        }
        else if(bro_checkNickname.StatusCode == 400)
        {
            Debug.Log("닉네임이 너무 깁니다. 다른 닉네임을 선택해주세요.");
        }
        else if (bro_checkNickname.StatusCode == 409)
        {
            Debug.Log("중복된 닉네임입니다. 다른 닉네임을 선택해주세요.");
        }
        else
        {
            Debug.Log("변경에 실패했습니다. 다시 시도해주세요.");
        }     
    }
}