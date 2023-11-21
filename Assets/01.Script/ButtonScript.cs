using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 땅 및 건물 구매창 UI 버튼 관리 스크립트
public class ButtonScript : MonoBehaviour
{
    GameManager theGM;
    public PurchaseSystem thePS;
    public GroundBuyScript theGBS;

    private void Start()
    {
        theGM = FindObjectOfType<GameManager>();
    }

    //Right버튼
    public void OnRightBtn()
    {
        thePS.cur += 1;
        // 커서가 리스트의 길이를 넘기면 0으로 초기화
        if (thePS.cur > theGM.buildings.Length - 1)
        {
            thePS.cur = 0;
        }
        thePS.buildingImg.sprite = theGM.buildings[thePS.cur].buildingImg.building_front;
        thePS.buildingText.text = theGM.buildings[thePS.cur].buildingName;
    }

    //Left버튼
    public void OnLeftBtn()
    {
        thePS.cur -= 1;
        // 커서가 0보다 작다면 마지막 건물 리스트로 초기화
        if (thePS.cur == -1)
        {
            thePS.cur = theGM.buildings.Length - 1;
        }
        thePS.buildingImg.sprite = theGM.buildings[thePS.cur].buildingImg.building_front;
        thePS.buildingText.text = theGM.buildings[thePS.cur].buildingName;
    }

    //건물 구매버튼
    public void OnPurchaseBtn()
    {
        thePS.BuildingPurchase();
    }

    //취소버튼
    public void OnPurchaseCloseBtn()
    {
        theGM.NextTurnFunc();
        theGM.UIFlag = false;
    }

    //땅 구매 버튼
    public void OnGroundBuyBtn()
    {
        theGBS.groundBuyFlag = true;
        theGM.nowPlayer.groundCount += 1;
        theGM.nowPlayer.playerMoney -= 50;

        // 땅만 샀을 경우 해당 타일을 추가하고 상대방이 밟았을 때 50골드만 감소시키는 것 구현 아직 안됨
        // theGM.nowPlayer.againstPlayer_Tile.Add(theGM.nowPlayer.nowTile.gameObject);
    }
}