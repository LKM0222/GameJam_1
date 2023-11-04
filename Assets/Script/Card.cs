using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

//카드의 정보를 담는 클래스
[System.Serializable]
public class Card
{
    public string card_name;
    public int cardCode;
    public Sprite cardImg;
    public GameObject targetTile; //건물 상호작용 필요 시 넣는 변수
}
