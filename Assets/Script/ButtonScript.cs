using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonScript : MonoBehaviour
{   
    public PurchaseSystem thePS;
    public GroundBuyScript theGBS;
    GameManager theGM;
    private void Start()
    {
        theGM = FindObjectOfType<GameManager>();
    }
    public void OnRightBtn(){//Right버튼
        thePS.cur += 1;
        if(thePS.cur > theGM.buildings.Length - 1){
            thePS.cur = 0;
        }
        thePS.buildingImg.sprite = theGM.buildings[thePS.cur].buildingImg.building_front;
        thePS.buildingText.text = theGM.buildings[thePS.cur].buildingName;
    }
    public void OnLeftBtn(){//Left버튼
        thePS.cur -= 1;
        if(thePS.cur == -1){
            thePS.cur = theGM.buildings.Length - 1;
        }
        thePS.buildingImg.sprite = theGM.buildings[thePS.cur].buildingImg.building_front;
        thePS.buildingText.text = theGM.buildings[thePS.cur].buildingName;
    }
    public void OnPurchaseBtn(){//건물 구매버튼
        thePS.buyFlag = true;
        theGM.nowPlayer.buildingCount += 1;
        theGM.nowPlayer.playerMoney -= 50;
    }
    public void OnPurchaseCloseBtn(){ //취소버튼
        theGM.turnCount += 1;
        theGM.nextTurn = true;
        theGM.UIFlag = false;
    }
    public void OnGroundBuyBtn(){ //땅 구매 버튼
        theGBS.groundBuyFlag = true;
        theGM.nowPlayer.groundCount += 1;
        theGM.nowPlayer.playerMoney -= 50;
        theGM.nowPlayer.againstPlayer_Tile.Add(theGM.nowPlayer.nowTile.gameObject);
    }
}
