using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BackEnd;

// 땅 및 건물 구매창 UI 버튼 관리 스크립트
public class ButtonScript : MonoBehaviour
{
    public PurchaseSystem purchase;
    public GroundBuyScript ground;

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
}