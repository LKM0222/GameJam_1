using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 토지 구매 스크립트
public class GroundBuyScript : MonoBehaviour
{
    public PurchaseSystem thePurchase;
    public GameObject purchaseUI;

    public void GroundBuy()
    {
        // 타일 소유주 변경 및 타일 가격 설정
        GameManager.Instance.nowPlayer.nowTile.ownPlayer = GameManager.Instance.nowPlayer.playerId;
        GameManager.Instance.nowPlayer.nowTile.price = 50;
        this.transform.parent.gameObject.SetActive(false);

        // 건물 구매 UI에 띄워줄 이미지, 건물 이름 초기화
        thePurchase.cur = 1;
        thePurchase.buildingImg.sprite = GameManager.Instance.buildings[1].buildingImg.building_front;
        thePurchase.buildingText.text = GameManager.Instance.buildings[1].buildingName;
        purchaseUI.SetActive(true);
    }
}