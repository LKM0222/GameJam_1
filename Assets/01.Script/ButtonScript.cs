using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BackEnd;

// 땅 및 건물 구매창 UI 버튼 관리 스크립트
public class ButtonScript : MonoBehaviour
{
    public PurchaseSystem purchase;
    public GroundBuyScript ground;

    [Header("TurnCard")]
    public int turnNum, turnCardIdx;

    private void Start()
    {
        purchase = FindObjectOfType<PurchaseSystem>();
        ground = FindObjectOfType<GroundBuyScript>();
    }

    public void RightButton()
    {
        AudioManager.Instance.Play("Click_Sound");

        purchase.cur += 1;

        if (purchase.cur > GameManager.Instance.buildings.Length - 1) purchase.cur = 1;

        purchase.buildingImg.sprite = GameManager.Instance.buildings[purchase.cur].buildingImg.building_front;
        purchase.buildingText.text = GameManager.Instance.buildings[purchase.cur].buildingName;
    }

    public void LeftButton()
    {
        AudioManager.Instance.Play("Click_Sound");

        purchase.cur -= 1;

        if (purchase.cur == 0) purchase.cur = GameManager.Instance.buildings.Length - 1;

        purchase.buildingImg.sprite = GameManager.Instance.buildings[purchase.cur].buildingImg.building_front;
        purchase.buildingText.text = GameManager.Instance.buildings[purchase.cur].buildingName;
    }

    public void PurchaseButton()
    {
        AudioManager.Instance.Play("Build_Sound");

        BuildingData bdata = new(purchase.cur);
        string jsonData = JsonUtility.ToJson(bdata);

        byte[] data = ParsingManager.Instance.ParsingSendData(ParsingType.BuildingBuy, jsonData);
        Backend.Match.SendDataToInGameRoom(data);

        purchase.BuildingPurchase();
    }

    public void CancelButton()
    {
        AudioManager.Instance.Play("Cancel_Sound");

        GameManager.Instance.UIFlag = false;

        byte[] data = ParsingManager.Instance.ParsingSendData(ParsingType.NextTurn, "");
        Backend.Match.SendDataToInGameRoom(data);
    }

    public void GroundBuyButton()
    {
        AudioManager.Instance.Play("Click_Sound");

        byte[] data = ParsingManager.Instance.ParsingSendData(ParsingType.GroundBuy, "");
        Backend.Match.SendDataToInGameRoom(data);

        ground.GroundBuy();
    }


    //게임시작때, 자기 턴을 클릭하는 버튼
    public void TurnCardClick()
    {
        AudioManager.Instance.Play("UseCard_Sound");

        if (GameManager.Instance.turnIndex == -1)
        { //묶어주지 않으면 계속 선택이 가능함...
            //턴 인덱스가 0이면 아직 내 턴카드를 선택하지 않은것.
            GameManager.Instance.turnIndex = turnNum; //나의 턴을 저장.

            if (GameManager.Instance.turnIndex == 1)
            {
                GameManager.Instance.myCharactor = GameObject.Find("Player1").GetComponent<PlayerManager>();
                // SessionData sessionData = new(GameManager.Instance.mySessionId, GameManager.Instance.turnIndex);
                SessionData sessionData = new(BackendManager.Instance.mySessionId, GameManager.Instance.turnIndex);
                string jsondata = JsonUtility.ToJson(sessionData);
                ParsingManager.Instance.ParsingSendData(ParsingType.Session, jsondata);
            }
            else
            {
                GameManager.Instance.myCharactor = GameObject.Find("Player2").GetComponent<PlayerManager>();
            }

            TurnCard tCard = new(turnCardIdx);
            string jsonData = JsonUtility.ToJson(tCard);
            byte[] data;
            data = ParsingManager.Instance.ParsingSendData(ParsingType.Turn, jsonData);
            Backend.Match.SendDataToInGameRoom(data);

            this.gameObject.SetActive(false);
        }
        GameManager.Instance.myCharactor.myTurnImg.SetActive(true);
    }
}