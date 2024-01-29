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
    [SerializeField] GameObject watingUI, turnCardUI;

    public void SetUI(){
       watingUI.SetActive(false);
       turnCardUI.SetActive(true); 
    }
}
