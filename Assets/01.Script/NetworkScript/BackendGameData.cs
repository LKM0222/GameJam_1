using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BackEnd;
using System.Text;
using System;
using System.Reflection;

public class UserData {
    public int winscore;
    public int losescore;
    public string nickname;
    public string email;

    public override string ToString()
    {   
        StringBuilder result = new StringBuilder();
        result.AppendLine($"winscore : {winscore}");
        result.AppendLine($"losescore : {losescore}");
        result.AppendLine($"nickname : {nickname}");
        result.AppendLine($"email : {email}");
        return base.ToString();
    }

}


public class BackendGameData
{
    private static BackendGameData _instance = null;
    public static BackendGameData Instance {
        get { 
            if(_instance == null){
                _instance = new BackendGameData();
            }
            return _instance;
        }
    }

    public static UserData userData;

    private string gamedataRowInDate = String.Empty;

    public void GameDataInsert(string _nickname, string _email) {
        if(userData == null){
            userData = new UserData();
        }
        Debug.Log("userdata를 초기화 합니다.");
        userData.losescore = 0;
        userData.winscore = 0;
        userData.nickname = _nickname;
        userData.email = _email;

        Param param = new Param();
        param.Add("winscore",userData.winscore);
        param.Add("losescore", userData.losescore);
        param.Add("nickname", userData.nickname);
        param.Add("email", userData.email);

        var bro = Backend.GameData.Insert("USER_DATA", param);

        if(bro.IsSuccess()){
            Debug.Log("데이터 삽입 성공 " + bro);
            gamedataRowInDate = bro.GetInDate();
        } else {
            Debug.Log("데이터 삽입 실패 " + bro);
        }
    }
    
    public void GameDataGet() {
        // Step 3. 게임정보 불러오기 구현하기
    }
    
    public void LevelUp() {
        // Step 4. 게임정보 수정 구현하기
    }
    
    public void GameDataUpdate() {
        // Step 4. 게임정보 수정 구현하기
    }

}
