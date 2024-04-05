using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// 건물 구매 스크립트
public class PurchaseSystem : MonoBehaviour
{
    public Text buildingText;
    public Image buildingImg;

    public int cur = 1;

    void Start()
    {
        buildingImg.sprite = GameManager.Instance.buildings[1].buildingImg.building_front;
        buildingText.text = GameManager.Instance.buildings[1].buildingName;
    }

    // 건물 건설 함수
    public void BuildingPurchase()
    {
        GameManager.Instance.nowPlayer.nowTile.building = GameManager.Instance.buildings[cur];
        GameManager.Instance.nowPlayer.nowTile.price = GameManager.Instance.buildings[cur].toll;
        GameManager.Instance.nowPlayer.playerMoney -= 50;
        GameManager.Instance.SetFloatingText(GameManager.Instance.nowPlayer, 50, false);

        this.transform.parent.gameObject.SetActive(false);
    }
}