using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using BackEnd;
using UnityEditor;

public class MenuSceneManager : MonoBehaviour
{
    [SerializeField] Text nickname;
    [SerializeField] Text winText;
    [SerializeField] Text loseText;
    [SerializeField] Text ratingText;

    private static MenuSceneManager _instance = null;
    public static MenuSceneManager Instance {
        get {
            if(_instance == null){
                _instance = new MenuSceneManager();
            }
            return _instance;
        }
    }


    private void Awake() {
        UserData user = BackendGameData.Instance.GameDataGet();
        print("user nickname : " + user.nickname);
        nickname.text = user.nickname;
        winText.text = "승리 : " + user.winscore.ToString();
        loseText.text = "패배 : " + user.losescore.ToString();
        if(user.winscore + user.losescore == 0){
            ratingText.text = "승률 : " + 0 + "%";
        }else{
            ratingText.text = "승률 : " + ( user.winscore / (user.winscore + user.losescore)) + "%";
        }
        
    }
    
    public void SetWinText(string _winText){
        winText.text = _winText;
    }
    public void SetLoseText(string _loseText){
        loseText.text = _loseText;
    }
    public void SetRatingText(){ //이 함수는 수정 필요함...
        UserData user = BackendGameData.Instance.GameDataGet();
        if(user.winscore + user.losescore == 0){
            ratingText.text = "승률 : " + 0 + "%";
        }else{
            ratingText.text = "승률 : " + ( user.winscore / (user.winscore + user.losescore)) + "%";
        }
    }

}
