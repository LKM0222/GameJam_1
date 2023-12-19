using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BackEnd;

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
            } else {
                Debug.Log("회원가입 실패! : " +bro);
            }
        }
    }

    public void CustomLogin(string id, string pw){

    }

    public void UpdateNickName(string nickname){

    }
}
