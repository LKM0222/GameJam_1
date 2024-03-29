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
    
    public UserData GameDataGet() {
        Debug.Log("게임 정보 불러오기");
        var bro = Backend.GameData.GetMyData("USER_DATA", new Where());
        if(bro.IsSuccess()){
            Debug.Log("게임 데이터 불러오기 성공! " + bro);

            LitJson.JsonData gameDataJson = bro.FlattenRows(); //Json으로 리턴한 데이터 받아오기.
            if(gameDataJson.Count <= 0){
                Debug.Log("데이터가 존재하지 않습니다.");
            }
            else{
                gamedataRowInDate = gameDataJson[0]["inDate"].ToString();

                userData = new UserData();
                userData.losescore = int.Parse(gameDataJson[0]["losescore"].ToString());
                userData.winscore = int.Parse(gameDataJson[0]["winscore"].ToString());
                userData.nickname = gameDataJson[0]["nickname"].ToString();
                userData.email = gameDataJson[0]["email"].ToString();

                Debug.Log(userData.ToString());
            }
            return userData;
        } else{
            Debug.LogError("게임 데이터 조회에 실패하였습니다. : " + bro);
            return null;
        }
    }

    public void UpWinScore() {
        Debug.Log("승리 포인트 증가");
        userData.winscore += 1;
    }
    public void UpLoseScore(){
        Debug.Log("패배 포인트 증가");
        userData.losescore += 1;
    }
    
    public void GameDataUpdate() {
        if(userData == null){
            Debug.Log("서버에서 다운받거나 새로 삽입한 데이터가 존재하지 않습니다. Insert 혹은 Get을 통해 데이터를 생성해주세요.");
            return;
        }
        Param param = new Param();
        param.Add("winscore",userData.winscore);
        param.Add("losescore", userData.losescore);
        param.Add("nickname", userData.nickname);
        param.Add("email", userData.email);

        BackendReturnObject bro = null;
        if(string.IsNullOrEmpty(gamedataRowInDate)){
            Debug.Log("내 제일 최신 게임정보 데이터 수정을 요청합니다.");
            bro = Backend.GameData.Update("USER_DATA", new Where(), param);
        } else { 
            Debug.Log($"{gamedataRowInDate}의 게임데이터 수정을 요청합니다.");
            bro = Backend.GameData.UpdateV2("USER_DATA",gamedataRowInDate, Backend.UserInDate, param);
        }

        if(bro.IsSuccess()){
            Debug.Log("게임정보 데이터 수정에 성공했습니다. : " + bro);
        } else{
            Debug.LogError("게임정보 데이터 수정에 실패했습니다. : " + bro);
        }
    }

}
