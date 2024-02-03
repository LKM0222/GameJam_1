using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// 건물 구매 관리 스크립트
public class PurchaseSystem : MonoBehaviour
{
    GameManager theGM;

    // 구매창 UI에 띄울 건물의 이름
    public Text buildingText;
    // 구매창 UI에 띄울 건물의 이미지
    public Image buildingImg;

    // 어떤 건물을 선택중인지 순서로 접근하는 변수
    public int cur = 1;

    //건물 구매 버튼을 눌렀을 경우 건물 구매를 위한 플래그
    public bool buyFlag;

    // Start is called before the first frame update
    void Start()
    {
        theGM = FindObjectOfType<GameManager>();
        buildingImg.sprite = theGM.buildings[1].buildingImg.building_front;
        buildingText.text = theGM.buildings[1].buildingName;
    }

    // Update is called once per frame
    void Update()
    {

    }

    // 빌딩 건설 함수
    public void BuildingPurchase()
    {
        theGM.nowPlayer.nowTile.building = theGM.buildings[cur]; //타일의 건물 정보를 현재 커서에서 선택한 건물로 변경

        this.transform.parent.gameObject.SetActive(false);//모든 작업 끝났으니 현재 UI종료

        theGM.nowPlayer.buildingCount += 1;
        theGM.nowPlayer.playerMoney -= 50;
        theGM.SetFloatingText(theGM.nowPlayer, 50, false);
        theGM.nowPlayer.nowTile.price = theGM.buildings[cur].toll;

        theGM.UIFlag = false; // UI를 끄기 위해 UIFlag False;
        theGM.NextTurnFunc(); //next turn 호출
    }
}