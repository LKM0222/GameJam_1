using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class AbilityManager : MonoBehaviour
{
    [Header("Ability")]
    [Header("Key Obj")]
    [SerializeField] GameObject zObj;
    [SerializeField] GameObject xObj;
    [SerializeField] GameObject cObj;
    [Header("Middle Obj")]
    [SerializeField] GameObject zMiddleObj;
    [SerializeField] GameObject xMiddleObj;
    [SerializeField] GameObject cMiddleObj;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {   
        try{
            //z키
            if(zMiddleObj.transform.childCount > 0){ //능력을 장착했다면 키에 이미지 생성
                zObj.GetComponent<Image>().color = zMiddleObj.transform.GetChild(0).GetComponent<Image>().color;
            }
            else{//키에 능력이 없다면
                zObj.GetComponent<Image>().color = Color.white;
            }
            //x키
            if(zMiddleObj.transform.childCount > 0){ //능력을 장착했다면 키에 이미지 생성
                xObj.GetComponent<Image>().color = xMiddleObj.transform.GetChild(0).GetComponent<Image>().color;
            }
            else{//키에 능력이 없다면
                xObj.GetComponent<Image>().color = Color.white;
            }
            //c키
            if(zMiddleObj.transform.childCount > 0){ //능력을 장착했다면 키에 이미지 생성
                cObj.GetComponent<Image>().color = cMiddleObj.transform.GetChild(0).GetComponent<Image>().color;
            }
            else{//키에 능력이 없다면
                cObj.GetComponent<Image>().color = Color.white; //원상태로 복귀, 위도 똑같음 또한 추후에 이미지가 생기면 추가로 작업
            }
        }catch(UnityException){

        }
        
    }
}
