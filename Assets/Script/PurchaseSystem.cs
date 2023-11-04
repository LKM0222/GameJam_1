using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PurchaseSystem : MonoBehaviour
{
    public Text buildingText; //건물의 이름 출력
    public Image buildingImg;

    public int cur = 0;
    GameManager theGM;
    public bool buyFlag; //건물 구매 버튼을 눌렀을 경우 건물 구매를 위한 플래그
    // Start is called before the first frame update
    void Start()
    {
        theGM = FindObjectOfType<GameManager>();
        buildingImg.sprite = theGM.buildings[0].buildingImg.building_front;
        buildingText.text = theGM.buildings[0].buildingName;
    }

    // Update is called once per frame
    void Update()
    {
        if(buyFlag){//구매 버튼을 눌렀다면
            theGM.nowPlayer.nowTile.building = theGM.buildings[cur]; //타일의 건물 정보를 현재 커서에서 선택한 건물로 변경
            theGM.UIFlag = false; //UI가 꺼졌으니 UIFlag False;
            buyFlag = false;//재반복 막기 위해 설정
            theGM.turnCount += 1; //건물까지 올렸으면 턴 끝남 1증가
            theGM.nextTurn = true;
            this.transform.parent.gameObject.SetActive(false);//모든 작업 끝났으니 현재 UI종료

        }
    }

    
}
