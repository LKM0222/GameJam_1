using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BackEnd;
using System.Runtime.InteropServices;
using BackEnd.Tcp;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine.SceneManagement;

public class BackendLogin
{
    private static BackendLogin _instance = null;
    public static BackendLogin Instance {
        get {
            if( _instance == null){
                _instance = new BackendLogin();
            }
            return _instance;
        }
    }
    
    public void CustomSignUp(string id, string pw, string nickname, string email){
        Debug.Log("회원가입을 요청합니다."); //아직까지는 문자열 사이의 공백은 신경쓰지 않고 있음. 추후 수정
        if(id.Trim() == "" || pw.Trim() == "" || nickname.Trim() == "" || email.Trim() == ""){ //
            Debug.Log("입력값이 없는 곳이 있습니다.");
        }
        else{
            var bro = Backend.BMember.CustomSignUp(id, pw);
            if(bro.IsSuccess()){
                Debug.Log("회원가입 성공! : " +bro);

                var Nemail = Backend.BMember.UpdateCustomEmail(email);
                if (Nemail.IsSuccess())Debug.Log("이메일 변경 로그" + Nemail);
                else Debug.Log("이메일 변경 실패");

                var Nnickname = Backend.BMember.UpdateNickname(nickname);
                if(Nnickname.IsSuccess()) {
                    Debug.Log("닉네임 변경 로그" + Nnickname);
                } else Debug.Log("닉네임 변경 실패");
                
                BackendGameData.Instance.GameDataInsert(nickname,email);
            } else {
                Debug.Log("회원가입 실패! : " +bro);
            }
        }
    }

    public void CustomLogin(string id, string pw){
        Debug.Log("로그인을 요청합니다.");
        var bro = Backend.BMember.CustomLogin(id,pw);

        if(bro.IsSuccess()){
            Debug.Log("로그인 성공!" + bro);
            // SceneManager.LoadScene("MenuScene");
            // // ErrorInfo errorInfo;
            // // Backend.Match.JoinMatchMakingServer(out errorInfo);
            // // Debug.Log("매치서버 접속 " + errorInfo);
            // // Backend.Match.OnJoinMatchMakingServer = (JoinChannelEventArgs args) => {
            // //     SceneManager.LoadScene("MenuScene");
            // // };
        }
        else {
            Debug.Log("로그인 실패" + bro);
        }
    }

    public void UpdateNickName(string nickname){

    }
}
