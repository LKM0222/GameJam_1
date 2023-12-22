using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using BackEnd;

public class MenuSceneManager : MonoBehaviour
{
    [SerializeField] Text nickname;
    [SerializeField] Text winText;
    [SerializeField] Text loseText;
    [SerializeField] Text ratingText;

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

    private void Start() {
    }

}
