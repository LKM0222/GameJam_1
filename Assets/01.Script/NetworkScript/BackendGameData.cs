using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BackEnd;
using System.Text;
using System;

public class UserData {
    public int winscore;
    public int losescore;
    public string nickname;

    public override string ToString()
    {   
        StringBuilder result = new StringBuilder();
        result.AppendLine($"winscore : {winscore}");
        result.AppendLine($"losescore : {losescore}");
        result.AppendLine($"nickname : {nickname}");
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

    public void GameDataInsert() {
        // Step 2. 게임정보 삽입 구현하기 
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
