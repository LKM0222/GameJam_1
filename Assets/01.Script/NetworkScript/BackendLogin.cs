using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;
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
    
    public int CustomSignUp(string id, string pw, string nickname, string email){
        //회원가입 요청
        if(id.Trim() == "" || pw.Trim() == "" || nickname.Trim() == "" || email.Trim() == ""){
            //입력값이 없을경우
            return 1;
        }
        else{
            string idChecker = Regex.Replace(id.Trim(), @"[^0-9a-zA-Z가-힣]{1,10}","",RegexOptions.Singleline);
            if(id.Equals(idChecker) == true){
                var bro = Backend.BMember.CustomSignUp(id, pw);
                if(bro.IsSuccess()){
                    //회원가입 성공
                    var Nemail = Backend.BMember.UpdateCustomEmail(email);

                    if (!Nemail.IsSuccess()) return 2; //이메일 변경 실패
                    
                    var Nnickname = Backend.BMember.UpdateNickname(nickname);
                    if(!Nnickname.IsSuccess()) return 3; //닉네임 변경 실패

                    BackendGameData.Instance.GameDataInsert(nickname,email);
                    return 4;
                } else return 5;//회원가입 실패
            }
            else return 6; //특수문자 포함된 아이디

        }
    }

    public void CustomLogin(string id, string pw){ //로그인 요청
        var bro = Backend.BMember.CustomLogin(id,pw);
        //변경실패는 bro.IsSuccess()
    }
}