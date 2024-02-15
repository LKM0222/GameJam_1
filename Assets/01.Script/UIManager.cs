using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    private static UIManager _instance;
    public static UIManager Instance{
        get{
            if(_instance == null){
                _instance = FindObjectOfType(typeof(UIManager)) as UIManager;
            }
            return _instance;
        }
    }
    [SerializeField] GameObject watingUI, turnCardUI, gameoverUI;

    public void SetUI(){
       watingUI.SetActive(false);
       turnCardUI.SetActive(true); 
    }

    //오류 났을때, 이 함수를 실행시켜서 나가기 버튼을 누를 수 있게 함.
    public void SetErrorUI(){
        watingUI.SetActive(false);
        turnCardUI.SetActive(false);
        gameoverUI.SetActive(true);
    }
}
