using System.Collections;
using System.Collections.Generic;
using LitJson;
using UnityEngine;
using BackEnd;
using BackEnd.Tcp;

// 땅 및 건물 구매창 UI 버튼 관리 스크립트
public class ButtonScript : MonoBehaviour
{
    GameManager theGM;
    public PurchaseSystem thePS;
    public GroundBuyScript theGBS;

    [Header("TurnCard")]
    public int turnNum, turncardIdx;

    private void Start()
    {
        theGM = FindObjectOfType<GameManager>();
        thePS = FindObjectOfType<PurchaseSystem>();
    }

    //Right버튼
    public void OnRightBtn()
    {
        AudioManager.Instance.Play("Click_Sound");

        thePS.cur += 1;
        // 커서가 리스트의 길이를 넘기면 0으로 초기화
        if (thePS.cur > theGM.buildings.Length - 1)
        {
            thePS.cur = 1;
        }
        thePS.buildingImg.sprite = theGM.buildings[thePS.cur].buildingImg.building_front;
        thePS.buildingText.text = theGM.buildings[thePS.cur].buildingName;
    }

    //Left버튼
    public void OnLeftBtn()
    {
        AudioManager.Instance.Play("Click_Sound");

        thePS.cur -= 1;
        // 커서가 1보다 작다면 마지막 건물 리스트로 초기화
        if (thePS.cur == 0)
        {
            thePS.cur = theGM.buildings.Length - 1;
        }
        thePS.buildingImg.sprite = theGM.buildings[thePS.cur].buildingImg.building_front;
        thePS.buildingText.text = theGM.buildings[thePS.cur].buildingName;
    }

    //건물 구매버튼
    public void OnPurchaseBtn()
    {
        AudioManager.Instance.Play("Build_Sound");

        BuildingData bdata = new(thePS.cur);
        string jsonData = JsonUtility.ToJson(bdata);

        byte[] data = ParsingManager.Instance.ParsingSendData(ParsingType.BuildingBuy, jsonData);
        Backend.Match.SendDataToInGameRoom(data);

        thePS.BuildingPurchase();
    }

    //취소버튼
    public void OnPurchaseCloseBtn()
    {
        AudioManager.Instance.Play("Cancel_Sound");

        theGM.UIFlag = false;
        //턴그냥 넘김
        byte[] data = ParsingManager.Instance.ParsingSendData(ParsingType.NextTurn, "");
        Backend.Match.SendDataToInGameRoom(data);
    }

    //땅 구매 버튼
    public void OnGroundBuyBtn()
    {
        AudioManager.Instance.Play("Click_Sound");

        // theGM.SetFloatingText(theGM.nowPlayer, 50, false);

        byte[] data = ParsingManager.Instance.ParsingSendData(ParsingType.GroundBuy, "");
        Backend.Match.SendDataToInGameRoom(data);
        theGBS.GroundBuy();
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

            TurnCard tCard = new(turncardIdx);
            string jsonData = JsonUtility.ToJson(tCard);
            byte[] data;
            data = ParsingManager.Instance.ParsingSendData(ParsingType.Turn, jsonData);
            Backend.Match.SendDataToInGameRoom(data);

            this.gameObject.SetActive(false);
        }
        GameManager.Instance.myCharactor.myTurnImg.SetActive(true);
    }
}